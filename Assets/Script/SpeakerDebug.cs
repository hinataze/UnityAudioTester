using Photon.Voice.Unity;
using UnityEngine;

public class SpeakerDebug : MonoBehaviour
{
    private Speaker spk;

    void Start()
    {
        spk = GetComponent<Speaker>();
        if (spk != null)
        {
            Debug.Log("🎧 Speaker component found!");
        }
    }

    void Update()
    {
        if (spk != null && spk.IsPlaying)
        {
            Debug.Log("🔊 Speaker is playing remote voice!");
        }
    }
}