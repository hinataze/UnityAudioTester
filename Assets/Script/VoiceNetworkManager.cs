using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections; // ✅ This is required for IEnumerator!
using Photon.Voice.Unity;


public class VoiceNetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;



    void Awake()
    {
        if (FindObjectsOfType<VoiceNetworkManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        PhotonNetwork.NickName = "User_" + Random.Range(1000, 9999);  // or use player name input
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("✅ VoiceStartupCheck: Script is running!");

    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("VoiceRoom", new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        StartCoroutine(SpawnPlayer());
    }

    IEnumerator SpawnPlayer()
    {
        yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom);
        yield return new WaitForSeconds(0.3f); // Let Voice init finish

        Vector3 pos = new Vector3(Random.Range(-2, 2), 1, Random.Range(-2, 2));
        Color myColor = new Color(Random.value, Random.value, Random.value);
        object[] instantiationData = new object[] { myColor.r, myColor.g, myColor.b };

        GameObject player = PhotonNetwork.Instantiate("PLAYER", pos, Quaternion.identity, 0, instantiationData);

        // 🟡 Add this block:
        PhotonView view = player.GetComponent<PhotonView>();
        Recorder recorder = player.GetComponent<Recorder>();
        if (recorder != null)
        {
            if (!view.IsMine)
            {
                recorder.enabled = false;
                Debug.Log("🔇 Recorder DISABLED for remote player");
            }
            else
            {
                Debug.Log("🎙️ Recorder ENABLED for local player");
            }
        }

        Debug.Log("✅ Instantiated PLAYER at: " + pos + " | Owned: " + view.IsMine);
        Debug.Log("✅ " + PhotonNetwork.NickName + " spawned player at " + pos);
    }



}