using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Put this on a single persistent GameObject (e.g. a "Telemetry" object in your bootstrap scene,
/// marked DontDestroyOnLoad so it survives scene loads).
///
/// Usage from anywhere:
///   TelemetryLogger.Instance.LogUIInteraction("StartButton", "Click");
///   TelemetryLogger.Instance.BeginInteraction("LoadLevel2");
///   ... load the level ...
///   TelemetryLogger.Instance.EndInteraction("LoadLevel2");
///
/// Writes buffered CSV files to Application.persistentDataPath so the same code
/// works in the Editor, PC builds, and VR builds (Quest, etc.) with no extra permissions.
///   - Windows:  %userprofile%\AppData\LocalLow\<Company>\<Product>\Telemetry\
///   - Quest/Android: /sdcard/Android/data/<package>/files/Telemetry/ (pull via adb or SideQuest)
/// </summary>
public class TelemetryLogger : MonoBehaviour
{
    public static TelemetryLogger Instance { get; private set; }

    [Header("Platform tag written into every row. Leave blank to auto-detect VR vs PC.")]
    [SerializeField] private string platformOverride = "";

    [Header("How often (seconds) buffered rows get written to disk")]
    [SerializeField] private float flushIntervalSeconds = 5f;

    [Header("How often (seconds) a continuous FPS sample is recorded")]
    [SerializeField] private float fpsSampleIntervalSeconds = 1f;

    private string sessionId;
    private string platformTag;
    private string sessionFolder;
    private string uiPath, timingPath, fpsPath;
    private bool isInitialized = false;

    private readonly List<string> uiBuffer = new List<string>();
    private readonly List<string> timingBuffer = new List<string>();
    private readonly List<string> fpsBuffer = new List<string>();
    private readonly List<string> positionBuffer = new List<string>();
    private readonly List<string> mousePositionBuffer = new List<string>();
    private readonly object bufferLock = new object();

    // continuous FPS tracking
    private float fpsTimer;
    private int fpsFrameCount;
    private float fpsAccum;
    private float flushTimer;

    // per-named-interaction tracking (supports several different interactions timing concurrently)
    private class ActiveTiming
    {
        public float startTime;
        public float frameAccumTime;
        public int frameCount;
        public float minFps = float.MaxValue;
        public float maxFps = float.MinValue;
    }
    private readonly Dictionary<string, ActiveTiming> activeTimings = new Dictionary<string, ActiveTiming>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        // Note: the session/files are NOT created here. Call StartSession(playerId)
        // once you know who the tester is (e.g. after they submit an ID on your
        // login/ID-entry scene). If you forget to call it, the first log call
        // will auto-start a session with a random ID as a safety net.
    }

    /// <summary>
    /// Call once, when you know who the tester is - e.g. from the "Submit" button
    /// on your ID-entry scene: TelemetryLogger.Instance.StartSession(idInputField.text);
    /// Pass null/empty to fall back to an auto-generated ID.
    /// </summary>
    public void StartSession(string customId = null)
    {
        if (isInitialized)
        {
            Debug.LogWarning("[TelemetryLogger] StartSession called more than once - ignoring.");
            return;
        }

        string idPart = string.IsNullOrWhiteSpace(customId)
            ? "anon" + UnityEngine.Random.Range(1000, 9999)
            : SanitizeForFileName(customId);
        sessionId = idPart;
        platformTag = DeterminePlatformTag();

        sessionFolder = Path.Combine(Application.persistentDataPath, "Telemetry");
        Directory.CreateDirectory(sessionFolder);

        uiPath = Path.Combine(sessionFolder, $"ui_interactions_{sessionId}.csv");
        timingPath = Path.Combine(sessionFolder, $"interaction_timings_{sessionId}.csv");
        fpsPath = Path.Combine(sessionFolder, $"fps_log_{sessionId}.csv");

        WriteHeader(uiPath, "Timestamp,SessionId,Platform,ElementName,Action,Context");
        WriteHeader(timingPath, "Timestamp,SessionId,Platform,InteractionName,DurationMs,AvgFPS,MinFPS,MaxFPS");
        WriteHeader(fpsPath, "Timestamp,SessionId,Platform,InstantFPS,RollingAvgFPS");

        isInitialized = true;
        Debug.Log($"[TelemetryLogger] Session '{sessionId}' ({platformTag}) logging to {sessionFolder}");
    }

    private static string SanitizeForFileName(string s)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            s = s.Replace(c, '_');
        return s.Replace(" ", "_");
    }

    private string DeterminePlatformTag()
    {
        if (!string.IsNullOrEmpty(platformOverride)) return platformOverride;
        // XRSettings.isDeviceActive is true when a VR headset is actually driving the display.
        return UnityEngine.XR.XRSettings.isDeviceActive ? "VR" : "PC";
    }

    private void Update()
    {
        if (!isInitialized) return;

        float dt = Time.unscaledDeltaTime;
        if (dt <= 0f) return;
        float instantFps = 1f / dt;

        // continuous rolling average, sampled periodically rather than every frame
        fpsFrameCount++;
        fpsAccum += instantFps;
        fpsTimer += dt;
        if (fpsTimer >= fpsSampleIntervalSeconds)
        {
            float rollingAvg = fpsAccum / fpsFrameCount;
            Enqueue(fpsBuffer, $"{Timestamp()},{sessionId},{platformTag},{Num(instantFps)},{Num(rollingAvg)}");
            fpsTimer = 0f;
            fpsFrameCount = 0;
            fpsAccum = 0f;
        }

        // feed any currently-open interaction timers
        if (activeTimings.Count > 0)
        {
            foreach (var kv in activeTimings)
            {
                var t = kv.Value;
                t.frameAccumTime += dt;
                t.frameCount++;
                if (instantFps < t.minFps) t.minFps = instantFps;
                if (instantFps > t.maxFps) t.maxFps = instantFps;
            }
        }

        flushTimer += dt;
        if (flushTimer >= flushIntervalSeconds)
        {
            flushTimer = 0f;
            FlushAll();
        }
    }

    private static string Timestamp() => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

    private static string Csv(string field)
    {
        if (field == null) return "";
        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
            return "\"" + field.Replace("\"", "\"\"") + "\"";
        return field;
    }

    // Always formats with a '.' decimal point regardless of the device's region/locale settings,
    // so numbers never collide with the CSV's ',' delimiters (e.g. PT-PT formats 200.0 as "200,0").
    private static string Num(float value) => value.ToString("F1", CultureInfo.InvariantCulture);

    /// <summary>Log a one-off UI interaction: a button press, menu open/close, grab, gaze select, etc.</summary>
    public void LogUIInteraction(string action, string context = "")
    {
        if (!isInitialized) StartSession(null);
        Enqueue(uiBuffer, $"{Timestamp()},{sessionId},{platformTag},{Csv(action)},{Csv(context)}");
    }

    /// <summary>
    /// Start timing a named interaction (e.g. "LoadLevel2", "OpenInventory", "BootToMenu").
    /// Call EndInteraction with the same name when it finishes. Use this for both
    /// "how long did X take" and "what was the average FPS while X happened".
    /// </summary>
    public void BeginInteraction(string interactionName)
    {
        if (!isInitialized) StartSession(null);
        activeTimings[interactionName] = new ActiveTiming { startTime = Time.realtimeSinceStartup };
    }

    /// <summary>Stop timing and write a row with duration + avg/min/max FPS observed during the window.</summary>
    // public void EndInteraction(string interactionName)
    // {
    //     if (!activeTimings.TryGetValue(interactionName, out var t))
    //     {
    //         Debug.LogWarning($"[TelemetryLogger] EndInteraction('{interactionName}') had no matching BeginInteraction.");
    //         return;
    //     }
    //     activeTimings.Remove(interactionName);

    //     float durationMs = (Time.realtimeSinceStartup - t.startTime) * 1000f;
    //     float avgFps = t.frameCount > 0 ? t.frameCount / Mathf.Max(t.frameAccumTime, 0.0001f) : 0f;
    //     float minFps = t.minFps == float.MaxValue ? 0 : t.minFps;
    //     float maxFps = t.maxFps == float.MinValue ? 0 : t.maxFps;

    //     Enqueue(timingBuffer, $"{Timestamp()},{sessionId},{platformTag},{Csv(interactionName)},{Num(durationMs)},{Num(avgFps)},{Num(minFps)},{Num(maxFps)}");
    // }

    /// <summary>Stop timing and write a row with duration + avg/min/max FPS observed during the window.</summary>
    public void EndInteraction(string interactionName)
    {
        EndInteractionCore(interactionName, interactionName);
    }
 
    private void EndInteractionCore(string interactionKey, string labelToWrite)
    {
        if (!activeTimings.TryGetValue(interactionKey, out var t))
        {
            Debug.LogWarning($"[TelemetryLogger] EndInteraction('{interactionKey}') had no matching BeginInteraction.");
            return;
        }
        activeTimings.Remove(interactionKey);
 
        float durationMs = (Time.realtimeSinceStartup - t.startTime) * 1000f;
        float avgFps = t.frameCount > 0 ? t.frameCount / Mathf.Max(t.frameAccumTime, 0.0001f) : 0f;
        float minFps = t.minFps == float.MaxValue ? 0 : t.minFps;
        float maxFps = t.maxFps == float.MinValue ? 0 : t.maxFps;
 
        Enqueue(timingBuffer, $"{Timestamp()},{sessionId},{platformTag},{Csv(labelToWrite)},{Num(durationMs)},{Num(avgFps)},{Num(minFps)},{Num(maxFps)}");
    }

    // Safety net: if e.g. "WholeTest" or "LoadScene" is still running when the app
    // closes/pauses (tester force-quit, crash, forgot to call EndInteraction, etc.),
    // write a final row for it instead of silently losing that data.
    private void EndAllActiveInteractions(string reasonSuffix)
    {
        if (activeTimings.Count == 0) return;
        var names = new List<string>(activeTimings.Keys);
        foreach (var name in names)
        {
            EndInteractionCore(name, name + "_" + reasonSuffix);
        }
    }

    /// <summary>
    /// Log the player's world position. Call this periodically (e.g. from ControllerManager
    /// on a timer) to build a movement trail/heatmap you can compare between VR and PC testers.
    /// </summary>
    public void LogPosition(Vector3 position)
    {
        if (!isInitialized) StartSession(null);
        Enqueue(positionBuffer, $"{Timestamp()},{sessionId},{platformTag},{Num(position.x)},{Num(position.y)},{Num(position.z)}");
    }

    /// <summary>
    /// Log the mouse cursor's screen-space position (PC only). Screen width/height are recorded
    /// alongside each sample so you can normalize to 0-1 later even if testers used different
    /// monitor resolutions.
    /// </summary>
    public void LogMousePosition(Vector2 screenPosition)
    {
        if (!isInitialized) StartSession(null);
        Enqueue(mousePositionBuffer, $"{Timestamp()},{sessionId},{platformTag},{Num(screenPosition.x)},{Num(screenPosition.y)},{Screen.width},{Screen.height}");
    }

    private void Enqueue(List<string> buffer, string row)
    {
        lock (bufferLock) buffer.Add(row);
    }

    private void WriteHeader(string path, string header)
    {
        if (!File.Exists(path)) File.WriteAllText(path, header + "\n", Encoding.UTF8);
    }

    private void FlushAll()
    {
        FlushBuffer(uiBuffer, uiPath);
        FlushBuffer(timingBuffer, timingPath);
        FlushBuffer(fpsBuffer, fpsPath);
    }

    private void FlushBuffer(List<string> buffer, string path)
    {
        string[] rows;
        lock (bufferLock)
        {
            if (buffer.Count == 0) return;
            rows = buffer.ToArray();
            buffer.Clear();
        }
        try
        {
            File.AppendAllText(path, string.Join("\n", rows) + "\n", Encoding.UTF8);
        }
        catch (Exception e)
        {
            Debug.LogError($"[TelemetryLogger] Failed writing {path}: {e.Message}");
        }
    }

    // Mobile/VR apps get paused (not always quit) when the user removes the headset
    // or the app loses focus, so flush there too, not just on quit.
    private void OnApplicationPause(bool paused)
    {
        if (paused) FlushAll();
    }

    private void OnApplicationQuit()
    {
        EndAllActiveInteractions("AppQuit");
        FlushAll();
    }
}