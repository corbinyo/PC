using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class pcInteraction : MonoBehaviourPun
{
    
    public PhotonView myPV;
    public bool isActiveToPlay;
    public checkInside checkInsideSphere;
    [PunRPC]
    private void Start()
    {
        this.GetComponent<MeshRenderer>().material.color = Color.red;
        myPV = GetComponent<PhotonView>();
    }
    public void PlayBell(string note)
    {
      
        myPV = GetComponent<PhotonView>();
      
    }

    [PunRPC]
   public void StopBell(string note)
    {
       
    
        myPV = GetComponent<PhotonView>();
       
}
    [PunRPC]
    public void RPC_setToPlay()
    {
        myPV.GetComponent<MeshRenderer>().material.color = Color.green;
        isActiveToPlay = true;
        checkInsideSphere.sequencerBoxActive = true;
      
    }
    [PunRPC]
    public void RPC_setToStop()
    {
        myPV.GetComponent<MeshRenderer>().material.color = Color.red;
        checkInsideSphere.sequencerBoxActive = false;
        isActiveToPlay = false;
    }

    [PunRPC]
    void RPC_SetColor(Color transferredColor)
    {
        gameObject.GetComponentInChildren<SpriteRenderer>().color = transferredColor;
    }


public void SoundYes()
    {
        Debug.Log("is this a thing?");
        this.photonView.RPC("PlayBell", RpcTarget.All);
        
    }
    public void SoundNo()
        {
        Debug.Log("is this a thing?");
        this.photonView.RPC("StopBell", RpcTarget.All);
            
        }
}
