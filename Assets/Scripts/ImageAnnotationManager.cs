using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Linq;
public class ImageAnnotationManager : MonoBehaviour
{
    private List<NoteData> allNotes;
    private ImageKey pendingKey;
    private Vector2 pendingPos;
    private string pendingMessage;
    [SerializeField] GameObject inputPanel;
    [SerializeField] GameObject inputPanelVR;
    private GameObject activePanel;
    private bool isVR;
    private ImageDisplayController pendingCaller;
    private string SavePath => System.IO.Path.Combine(Application.persistentDataPath, "annotations.json");
    private NoteData editingNote = null;

    private void Awake()
    {
        allNotes = new List<NoteData>();
        LoadNotes();
    }

    void Start()
    {
        XRModeSwitcher.OnModeSelected += OnModeChosen;
    }

    public void OnModeChosen(bool isVR)
    {
        Debug.Log("VR" + isVR);
        this.isVR = isVR;
        if (isVR)
            activePanel = inputPanelVR;
        else
            activePanel = inputPanel;
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
        pendingMessage = "";
        activePanel.SetActive(true);
    }

    public void OpenNoteInput(ImageKey key, Vector2 relativePos, ImageDisplayController caller, string oldNote)
    {
        pendingKey = key;
        pendingPos = relativePos;
        pendingCaller = caller;
        pendingMessage = oldNote;
        activePanel.SetActive(true);
    }

    public ImageDisplayController GetPendingCaller()
    {
        return pendingCaller;
    }

    public string GetPendingMessage()
    {
        return pendingMessage;
    }

    public void ClearNotesForImage(ImageKey key)
    {
        allNotes.RemoveAll(n => n.imageKey.Equals(key));
        SaveNotes();
    }

    public void ClearAllNotes()
    {
        allNotes.Clear();
        SaveNotes();
    }

    public bool IsInputPanelOpen()
    {
        return activePanel != null && activePanel.activeSelf;
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

    private System.Action onConfirmCallback;

    public void EditNote(NoteData note, ImageDisplayController displayController, System.Action onConfirm = null)
    {
        editingNote = note;
        onConfirmCallback = onConfirm;
        OpenNoteInput(note.imageKey, note.relativePos, displayController, note.message);
    }

    public NoteData ConfirmNote(string message)
    {
        NoteData note;

        if (editingNote != null)
        {
            editingNote.message = message;
            editingNote.created = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm");
            note = editingNote;
            editingNote = null;
            SaveNotes();
        }
        else
        {
            note = CreateNote(pendingKey, pendingPos, message);
        }

        activePanel.SetActive(false);
        onConfirmCallback?.Invoke();
        onConfirmCallback = null;
        return note;
    }

    public void DeleteNote(NoteData note)
    {
        allNotes.Remove(note);
        SaveNotes();
    }
}

[System.Serializable]
public class NoteDataList
{
    public List<NoteData> notes;
}
