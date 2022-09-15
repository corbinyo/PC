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
    private int mySeqBoxPVInt;
    void Start()
    {
        myPV = GetComponent<PhotonView>();
        mySeqBoxPVInt = sequencerBox.GetComponent<PhotonView>().ViewID;

    }

    public void SphereColliderOn()
    {
        Debug.Log("is this a thing?");
        m_Collider.enabled = true;
    }


    [PunRPC]
    public void RPC_OnTriggerCollideWithDodec()
    {
        Debug.Log("RPC called on checkINside Script");
        PhotonView.Find(mySeqBoxPVInt).RPC("RPC_setToPlay", RpcTarget.AllBuffered);

    }

    [PunRPC]
    void RPC_OnTriggerExitCollideWithDodec()
    {
        PhotonView.Find(mySeqBoxPVInt).RPC("RPC_setToStop", RpcTarget.AllBuffered);

    }
}
 