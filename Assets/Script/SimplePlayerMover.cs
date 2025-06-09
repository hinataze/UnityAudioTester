using UnityEngine;
using Photon.Pun;

public class SimplePlayerMover : MonoBehaviourPun
{
    public float moveSpeed = 5f;

    void Start()
    {
        Debug.Log("PLAYER prefab created on: " + PhotonNetwork.NickName);
        Debug.Log("SimplePlayerMover active on " + gameObject.name + ", isMine=" + photonView.IsMine);
    }

    void Update()
    {
        if (!photonView.IsMine) return; // Only move your own cube

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);
    }
}