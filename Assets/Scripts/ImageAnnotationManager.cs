using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Linq;
public class ImageAnnotationManager : MonoBehaviour
{
    private List<NoteData> allNotes;
    private ImageKey pendingKey;
    private Vector2 pendingPos;
    [SerializeField] GameObject inputPanel;
    private ImageDisplayController pendingCaller;
    private string SavePath => System.IO.Path.Combine(Application.persistentDataPath, "annotations.json");

    private void Awake()
    {
        allNotes = new List<NoteData>();
        LoadNotes();
    }
    private void LoadNotes()
    {
        if (!System.IO.File.Exists(SavePath)) return;

        string json = System.IO.File.ReadAllText(SavePath);
        NoteDataList wrapper = JsonUtility.FromJson<NoteDataList>(json);
        if (wrapper != null && wrapper.notes != null)
            allNotes = wrapper.notes;
    }
    public void ExportReport()
    {
        string report = GenerateReport();
        string path = System.IO.Path.Combine(Application.persistentDataPath, "annotations_report.txt");
        System.IO.File.WriteAllText(path, report);
        Debug.Log($"Report saved to: {path}");
    }

    private void SaveNotes()
    {
        NoteDataList wrapper = new NoteDataList { notes = allNotes };
        string json = JsonUtility.ToJson(wrapper, true);
        System.IO.File.WriteAllText(SavePath, json);
    }

    public string GenerateReport()
    {
        var sb = new StringBuilder();
        foreach (var note in allNotes.GroupBy(n => n.imageKey))
        {
            sb.AppendLine($"=== {note.Key} ===");
            foreach (var n in note)
                sb.AppendLine($"{n.created} — {n.message}");
        }
        return sb.ToString();
    }

    public List<NoteData> GetNotesForImage(ImageKey key)
    {
        return allNotes.Where(n => n.imageKey.Equals(key)).ToList();
    }

    public void OpenNoteInput(int year, int hotspotId, string direction, Vector2 relativePos, ImageDisplayController caller)
    {
        pendingKey = new ImageKey { year = year, hotspotId = hotspotId, direction = direction };
        pendingPos = relativePos;
        pendingCaller = caller;
        inputPanel.SetActive(true);
    }

    public NoteData ConfirmNote(string message)
    {
        NoteData note = CreateNote(pendingKey, pendingPos, message);
        inputPanel.SetActive(false);
        return note;
    }

    public ImageDisplayController GetPendingCaller()
    {
        return pendingCaller;
    }

    private NoteData CreateNote(ImageKey key, Vector2 relPosition, string message)
    {
        NoteData note = new NoteData
        {
            imageKey = key,
            relativePos = relPosition,
            message = message,
            created = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm")
        };
        allNotes.Add(note);
        SaveNotes();
        return note;
    }
}

[System.Serializable]
public class NoteDataList
{
    public List<NoteData> notes;
}
