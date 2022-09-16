using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


public class checkInside : MonoBehaviour
{
    //Make sure to assign this in the Inspector window
    public GameObject sequencerBox;
    Collider m_Collider;
    Vector3 m_Point;
    private Rigidbody rb;
    public bool sequencerBoxActive;
    public string currentNote;
    private PhotonView myPV;
    private PhotonView mySeqBoxPVInt;
    //private int mySeqBoxPVInt;
    void Start()
    {
        myPV = GetComponent<PhotonView>();
        mySeqBoxPVInt = sequencerBox.GetComponent<PhotonView>();


    }

    public void SphereColliderOn()
    {
       // Debug.Log("is this a thing?");
       m_Collider.enabled = true;
    }


    [PunRPC]
    public void RPC_OnTriggerCollideWithDodec()
    {

        //calls RPC_setToPlay on PCINTEREACTION
        //PhotonView.Find(mySeqBoxPVInt).RPC("RPC_setToPlay", RpcTarget.AllBuffered);
        mySeqBoxPVInt.RPC("RPC_setToPlay", RpcTarget.All);
        Debug.Log("RPC called on PCINTERACTION Script");

    }

    [PunRPC]
    void RPC_OnTriggerExitCollideWithDodec()
    {
        Debug.Log("RPC called on PCINTERACTION Script");
        // PhotonView.Find(mySeqBoxPVInt).RPC("RPC_setToStop", RpcTarget.All);
        mySeqBoxPVInt.RPC("RPC_setToStop", RpcTarget.All);

    }
}
 