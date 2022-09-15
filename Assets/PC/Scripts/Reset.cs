using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Reset : MonoBehaviour
{
    private PhotonView myPV;
    public int level;
    Scene m_Scene;
    string sceneName;
   
    void Start()
    {
       
        myPV = GetComponent<PhotonView>();
        PhotonView.DontDestroyOnLoad(myPV);
        //if (sceneName == "Restart")
        //{
        //    Destroy(myPV);
        //    PhotonNetwork.LoadLevel(level);
        //}


    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
       
            myPV.RPC("Restart", RpcTarget.All);
        }

    }
    [PunRPC]
    void Restart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
         
            PhotonNetwork.LoadLevel(level);
        }
    }

    }