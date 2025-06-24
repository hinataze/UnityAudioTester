using System.IO;
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class AudioPluginProfilerLogger : MonoBehaviour
{
    public AudioSource audioSource;
    public Transform emitter;
    public float testDurationSeconds = 10f;

    private Transform listener;
    private string logPath;
    private string profilerPath;
    private float startTime;

    void Start()
    {
        listener = FindObjectOfType<AudioListener>()?.transform;

        string plugin = AudioSettings.GetSpatializerPluginName();
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string folderPath = Application.isEditor
    ? Application.dataPath + "/../AudioPlugInTestLogs"
    : Path.Combine(Application.persistentDataPath, "AudioPlugInTestLogs");
        Directory.CreateDirectory(folderPath);

        logPath = Path.Combine(folderPath, $"audio_log_{plugin}_{timestamp}.csv");
        profilerPath = Path.Combine(folderPath, $"profiler_{plugin}_{timestamp}.data");
        startTime = Time.time;

        string clipName = audioSource?.clip?.name ?? "(none)";
        int clipFrequency = audioSource?.clip?.frequency ?? 0;
        int clipChannels = audioSource?.clip?.channels ?? 0;

        string metadata =
            $"# Spatializer Plugin: {plugin}\n" +
            $"# Timestamp: {timestamp}\n" +
            $"# Scene: {sceneName}\n" +
            $"# Listener: {GetHierarchyPath(listener)} at {listener?.position}\n" +
            $"# Emitter: {GetHierarchyPath(emitter)} at {emitter?.position}\n" +
            $"# AudioSource: {GetHierarchyPath(audioSource?.transform)}\n" +
            $"# AudioClip: {clipName}\n" +
            $"# SampleRate: {clipFrequency}, Channels: {clipChannels}\n" +
            $"# AudioSource Volume: {audioSource?.volume:F2}, Pitch: {audioSource?.pitch:F2}, SpatialBlend: {audioSource?.spatialBlend:F2}\n" +
            $"# RolloffMode: {audioSource?.rolloffMode}, MinDist: {audioSource?.minDistance}, MaxDist: {audioSource?.maxDistance}\n";

        File.WriteAllText(logPath, metadata + "\n");

        File.AppendAllText(logPath,
            "Time(s),CPU_BufferTime(ms),Distance(m),Gain,Pitch,SpatialBlend,RolloffMode,ActiveEmitters,CumulativeGain,PlayingEmitters\n");

        Debug.Log("[AudioProfiler] Metadata written:\n" + metadata);

#if UNITY_EDITOR
        ProfilerDriver.enabled = true;
        Debug.Log("[AudioProfiler] Profiler started. Awaiting Stop call...");
#endif

    }

    void LateUpdate()
    {
        if (listener == null || audioSource == null || emitter == null) return;

        float time = Time.time - startTime;
        float distance = Vector3.Distance(listener.position, emitter.position);

        AudioSettings.GetDSPBufferSize(out int bufferLength, out int numBuffers);
        float bufferMs = ((float)bufferLength / AudioSettings.outputSampleRate) * 1000f;

        // 🔎 Additional metrics
        AudioSource[] allSources = FindObjectsOfType<AudioSource>();
        int totalEmitters = allSources.Length;
        int playingEmitters = 0;
        float cumulativeGain = 0f;

        foreach (var src in allSources)
        {
            if (src.isPlaying)
            {
                cumulativeGain += src.volume;
                playingEmitters++;
            }
        }

        string line = string.Format("{0:F2},{1:F3},{2:F2},{3:F2},{4:F2},{5:F2},{6},{7},{8:F2},{9}\n",
            time,
            bufferMs,
            distance,
            audioSource.volume,
            audioSource.pitch,
            audioSource.spatialBlend,
            audioSource.rolloffMode,
            totalEmitters,
            cumulativeGain,
            playingEmitters);

        File.AppendAllText(logPath, line);
    }

#if UNITY_EDITOR
    private IEnumerator StopProfilerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StopAndSaveProfiler();
    }

    public void StopAndSaveProfiler()
    {
#if UNITY_EDITOR
        if (ProfilerDriver.enabled)
        {
            ProfilerDriver.enabled = false;
            ProfilerDriver.SaveProfile(profilerPath);
            Debug.Log("[AudioProfiler] Profiler saved to: " + profilerPath);

            // Exit play mode after saving
            EditorApplication.delayCall += () =>
            {
                Debug.Log("[AudioProfiler] Exiting Play Mode now.");
                EditorApplication.isPlaying = false;
            };
        }
#endif
    }
#endif

    string GetHierarchyPath(Transform t)
    {
        if (t == null) return "NULL";
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + " > " + path;
        }
        return path;
    }
}