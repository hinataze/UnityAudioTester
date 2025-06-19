using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Collections;
#endif

public class ListenerPathMover : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float pauseTime = 1f;
    public GameObject audioSourceToDuplicate;  // Original audio source to clone
    public Transform duplicateParent;          // Optional: parent for clones
    public int maxLoops = 2;                  // Limit to 10 loops

    private int currentWaypoint = 0;
    private float pauseTimer = 0f;
    private bool justLooped = false;
    private int completedLoops = 0;
    private bool isActive = true;

    void Start()
    {
        if (waypoints.Length > 0)
            transform.position = waypoints[0].position;
    }


    void Update()
    {
        Debug.Log("ListenerPathMover.Update() called");

        // Emitter diagnostics
        AudioSource[] allSources = FindObjectsOfType<AudioSource>();
        int totalEmitters = allSources.Length;
        int playingEmitters = 0;
        float cumulativeGain = 0f;

        foreach (var src in allSources)
        {
            if (src.isPlaying)
            {
                playingEmitters++;
                cumulativeGain += src.volume;
            }
        }

        Debug.Log($"[AudioProfiler] Emitters: {totalEmitters}, Playing: {playingEmitters}, Cumulative Gain: {cumulativeGain:F2}, Completed Loops: {completedLoops}");

        if (!isActive || waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypoint];
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        // Arrived at waypoint
        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= pauseTime)
            {

                int nextWaypoint = (currentWaypoint + 1) % waypoints.Length;

                Debug.Log($"[AudioProfiler] current: {currentWaypoint}, next: {nextWaypoint}, justLooped: {justLooped}, completedLoops: {completedLoops}");


                // Completed a full loop
                if (nextWaypoint == 0 && !justLooped)
                {
                    DuplicateAudioSource();
                    completedLoops++;
                    justLooped = true;
                    Debug.Log($"[LoopDebug] Update Tick | currentWaypoint: {currentWaypoint}, justLooped: {justLooped}, completedLoops: {completedLoops}, maxLoops {maxLoops}, isActive: {isActive}");


                    if (completedLoops >= maxLoops)
                    {
                        Debug.Log($"[AudioProfiler] Max loop count ({maxLoops}) reached. Stopping movement.");
                        isActive = false;

#if UNITY_EDITOR
                        var logger = FindObjectOfType<AudioPluginProfilerLogger>();
                        if (logger != null)
                        {
                            logger.StopAndSaveProfiler();
                        }
                        else
                        {
                            Debug.LogWarning("[AudioProfiler] Could not find AudioPluginProfilerLogger.");
                        }

                        Debug.Log($"[AudioProfiler] Final Loop #{completedLoops} | Emitters: {totalEmitters}");
                        Debug.Log("[AudioProfiler] Scheduling exit from Play Mode...");
                        EditorApplication.delayCall += () =>
                        {
                            Debug.Log("[AudioProfiler] Exiting Play Mode now.");
                            EditorApplication.isPlaying = false;
                        };
#endif
                        return;
                    }
                }

                // Reset justLooped if progressing through path
                if (currentWaypoint != 0)
                {
                    justLooped = false;
                    Debug.Log($"[AudioProfiler] Moved to waypoint {currentWaypoint}. justLooped is now {justLooped}");
                }

                currentWaypoint = nextWaypoint;
                pauseTimer = 0f;
            }
        }
    }
        

    void DuplicateAudioSource()
    {
        if (audioSourceToDuplicate != null)
        {
            GameObject newSource = Instantiate(audioSourceToDuplicate, audioSourceToDuplicate.transform.position, audioSourceToDuplicate.transform.rotation);
            if (duplicateParent != null)
                newSource.transform.SetParent(duplicateParent);

            // Count current active emitters
            int emitterCount = FindObjectsOfType<AudioSource>().Length;

            Debug.Log($"[AudioProfiler] Loop #{completedLoops + 1} | Total Emitters: {emitterCount}");
        }
    }


#if UNITY_EDITOR
    private IEnumerator StopProfilerAndExit()
    {
        var logger = FindObjectOfType<AudioPluginProfilerLogger>();
        if (logger != null)
        {
            logger.StopAndSaveProfiler();
            Debug.Log("[AudioProfiler] Waiting to exit...");
            yield return new WaitForSeconds(0.5f);  // Allow profiler flush
        }
        else
        {
            Debug.LogWarning("[AudioProfiler] Could not find logger.");
        }

        EditorApplication.isPlaying = false;
    }
#endif
}