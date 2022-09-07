using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class pcInteraction : MonoBehaviourPun
{
    
    private PhotonView myPV;
    public bool isActiveToPlay;
    public checkInside checkInsideSphere;
    [PunRPC]
    private void Start()
    {
        this.GetComponent<MeshRenderer>().material.color = Color.red;
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

    public void setToPlay()
    {
        this.GetComponent<MeshRenderer>().material.color = Color.green;
        isActiveToPlay = true;
        checkInsideSphere.sequencerBoxActive = true;
      
    }
    public void setToStop()
    {
        this.GetComponent<MeshRenderer>().material.color = Color.red;
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
        
        this.photonView.RPC("PlayBell", RpcTarget.All);
        
    }
    public void SoundNo()
        {
      
        this.photonView.RPC("StopBell", RpcTarget.All);
            
        }
}
