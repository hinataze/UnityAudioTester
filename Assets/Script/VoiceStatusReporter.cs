using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;

public class VoiceStatusReporter : MonoBehaviourPunCallbacks
{
    void Start()
    {
        InvokeRepeating(nameof(PrintAllAudioReports), 2f, 5f); // Every 5 seconds
    }

    void PrintAllAudioReports()
    {
        Debug.Log("🔊🔍 AUDIO REPORT FOR ALL PLAYERS:");

        PhotonVoiceView[] allVoiceViews = FindObjectsOfType<PhotonVoiceView>();

        foreach (var vv in allVoiceViews)
        {
            PhotonView view = vv.GetComponent<PhotonView>();
            string playerName = view.IsMine ? PhotonNetwork.NickName : view.Owner?.NickName ?? "Unknown";

            Debug.Log($"🧍 AR  {playerName} (Actor#{view.OwnerActorNr})");

            if (vv.RecorderInUse != null)
            {
                Debug.Log($"🎙️  AR Mic Transmitting: {vv.RecorderInUse.IsCurrentlyTransmitting}");
                Debug.Log($"🎚️  AR Mic Device: {vv.RecorderInUse.MicrophoneDevice}");
            }

            if (vv.SpeakerInUse != null)
            {
                Debug.Log($"🔊  AR Receiving Audio: {vv.SpeakerInUse.IsPlaying}");
            }

            if (vv.RecorderInUse != null)
            {
                Debug.Log($"🎙️ AR Mic Transmitting: {vv.RecorderInUse.IsCurrentlyTransmitting}");
            }

            Debug.Log("------");
        }


    }
}