using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class restartPC : MonoBehaviour
{
    private PhotonView myPV;
    // Start is called before the first frame update
    void Start()
    {
        myPV = GetComponent<PhotonView>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            myPV.RPC("RestartGame", RpcTarget.All);
        }
    }
    [PunRPC]
    public void RestartGame()
    {
        //if (PhotonNetwork.IsMasterClient)
        //{
            PhotonNetwork.LoadLevel(0);
        //}
    }
}
