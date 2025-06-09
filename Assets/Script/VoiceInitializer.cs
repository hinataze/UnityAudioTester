using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

public class VoiceInitializer : MonoBehaviourPun
{
    private PhotonVoiceView voiceView;


void Update()
{
    if (!photonView.IsMine && voiceView.SpeakerInUse != null)
    {
        if (voiceView.SpeakerInUse.IsPlaying)
        {
            Debug.Log($"🔊 Initializer {photonView.Owner.NickName} is speaking (Speaker is playing)");
        }
    }

    if (photonView.IsMine)
    {
        if (voiceView.RecorderInUse != null)
        {
            Debug.Log($"🎙️ Initializer Local Transmitting: {voiceView.RecorderInUse.IsCurrentlyTransmitting}");
        }

        if (voiceView.SpeakerInUse != null)
        {
            Debug.Log($"🎧 Initializer Local Receiving Audio: {voiceView.SpeakerInUse.IsPlaying}");
        }
    }
}

    void Start()
    {
        voiceView = GetComponent<PhotonVoiceView>();

        if (voiceView == null)
        {
            Debug.LogError("❌ Initializer PhotonVoiceView is missing!");
            return;
        }

        // ✅ Local player mic setup
        if (photonView.IsMine)
        {
                Debug.Log("🎙️ Initializer Mic");

            Recorder recorder = voiceView.RecorderInUse;
            if (recorder != null)
            {
                recorder.TransmitEnabled = true;
                recorder.RecordingEnabled = true;

                Debug.Log("🎙️ Initializer Recorder enabled and transmitting for local player");
            }
            else
            {
                Debug.LogWarning("⚠️ Initializer RecorderInUse not found on local player!");
            }
        }

        // ✅ All players: make sure Speaker is working
        Speaker speaker = voiceView.SpeakerInUse;
        if (speaker != null)
        {

                Debug.Log("🎙️ Initializer Speaker");

            if (speaker.IsPlaying)
            {
                Debug.Log("🔊 Initializer Speaker is actively playing remote voice");
            }
            else
            {
                Debug.Log("🕸️ Initializer Speaker assigned but not playing yet");
            }
        }
        else
        {
            Debug.LogWarning("🔇 Initializer Speaker not found or not yet initialized");
        }
    }
}