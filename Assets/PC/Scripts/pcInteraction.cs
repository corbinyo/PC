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
    public Material green;
    public Material red;
    [PunRPC]
    private void Start()
    {
      
        myPV = GetComponent<PhotonView>();
        myPVInt = this.myPV.ViewID;
        allPCInteraction = myPV.gameObject.GetComponent<pcInteraction>();
   
      
    }
    public void PlayBell(string note)
    {
      
        myPV = GetComponent<PhotonView>();
      
    }

  
   public void StopBell(string note)
    {
       
    
        myPV = GetComponent<PhotonView>();
       
}

    [PunRPC]
    public void RPC_setToPlay()
    {
     
            Debug.Log("is this a thing? green. LAST STOP BEFORE SEQUENCER");
            allPCInteraction.allSequencerBoxMesh.material = green;
            allPCInteraction.isActiveToPlay = true;
            allPCInteraction.allCheckInsideSphere.sequencerBoxActive = true;
        }
    
    [PunRPC]
    public void RPC_setToStop()
    {
      
            Debug.Log("is this a thing? red. LAST STOP BEFORE SEQUENCER");
            allPCInteraction.allSequencerBoxMesh.material = red;
            allPCInteraction.isActiveToPlay = false;
            allPCInteraction.allCheckInsideSphere.sequencerBoxActive = false;
        }
    

 
public void SoundYes()
    {
        Debug.Log("is this a thing?");
        myPV.RPC("PlayBell", RpcTarget.All);
        
    }
    public void SoundNo()
        {
        Debug.Log("is this a thing?");
        myPV.RPC("StopBell", RpcTarget.All);
            
        }
}
