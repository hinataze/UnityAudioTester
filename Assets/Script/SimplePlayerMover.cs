using UnityEngine;
using Photon.Pun;

public class SimplePlayerMover : MonoBehaviourPun
{

    public float moveSpeed = 5f;

    void Start()
    {
        Renderer rend = GetComponentInChildren<Renderer>();

        if (photonView.InstantiationData != null && photonView.InstantiationData.Length == 3)
        {
            float r = (float)photonView.InstantiationData[0];
            float g = (float)photonView.InstantiationData[1];
            float b = (float)photonView.InstantiationData[2];

            Color receivedColor = new Color(r, g, b);

            if (rend != null)
            {
                rend.material.color = receivedColor;
            }

            Debug.Log("🎨 Color applied from RGB: " + receivedColor);
        }
        else
        {
            Debug.LogWarning("⚠️ Missing or incomplete InstantiationData.");
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);
    }
}