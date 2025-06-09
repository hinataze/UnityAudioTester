using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class VoiceNetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;

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
        Debug.Log("✅ Joined room, now instantiating player!");


        Vector3 pos = new Vector3(Random.Range(-2, 2), 1, Random.Range(-2, 2));
        PhotonNetwork.Instantiate("PLAYER", pos, Quaternion.identity);
    }


}