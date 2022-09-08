using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class pcInteraction : MonoBehaviourPun
{
    
    public PhotonView myPV;
    public bool isActiveToPlay;
    private int myPVInt;
    public checkInside allCheckInsideSphere;
    public pcInteraction allPCInteraction;
    public MeshRenderer allSequencerBoxMesh;

    [PunRPC]
    private void Start()
    {
      
        myPV = GetComponent<PhotonView>();
        myPVInt = this.myPV.ViewID;
        allPCInteraction = PhotonView.Find(myPVInt).gameObject.GetComponent<pcInteraction>();
   
        allPCInteraction.allSequencerBoxMesh.material.color = Color.red;
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
        Debug.Log("is this a thing? green");
        allPCInteraction.allSequencerBoxMesh.material.color = Color.green;
        allPCInteraction.isActiveToPlay = true;
        allPCInteraction.allCheckInsideSphere.sequencerBoxActive = true;
      
    }
    [PunRPC]
    public void RPC_setToStop()
    {
        Debug.Log("is this a thing? red");
        allPCInteraction.allSequencerBoxMesh.material.color = Color.red;
        allPCInteraction.isActiveToPlay = false;
        allPCInteraction.allCheckInsideSphere.sequencerBoxActive = false;
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
