using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


public class checkInside : MonoBehaviour
{
    //Make sure to assign this in the Inspector window
    public pcInteraction pc;
    Collider m_Collider;
    Vector3 m_Point;
    private Rigidbody rb;
    public bool sequencerBoxActive;
    public string currentNote;
    private PhotonView myPV;
    void Start()
    {
        myPV = GetComponent<PhotonView>();
        m_Collider = GetComponent<Collider>();
       
    }

    public void SphereColliderOn()
    {
        Debug.Log("is this a thing?");
        m_Collider.enabled = true;
    }


    [PunRPC]
    void RPC_OnTriggerCollideWithDodec(int viewID)
    {
        pc.myPV.RPC("RPC_setToPlay", RpcTarget.All, viewID);

    }

    [PunRPC]
    void RPC_OnTriggerExitCollideWithDodec(int viewID)
    {
        pc.myPV.RPC("RPC_setToStop", RpcTarget.All, viewID);

    }
    void OnTriggerEnter(Collider collision)

    {

        if (collision.gameObject.tag == "sec")
        {
            int viewIDOfObject = collision.GetComponent<PhotonView>().ViewID;

            this.myPV.RPC("RPC_OnTriggerCollideWithDodec", RpcTarget.All, viewIDOfObject);
            Debug.Log("make box active");
       
        }
    }

    void OnTriggerExit(Collider collision)

    {

        if (collision.gameObject.tag == "sec")
        {
            int viewIDOfObject = collision.GetComponent<PhotonView>().ViewID;
            Debug.Log("make box inactive");
            this.myPV.RPC("RPC_OnTriggerExitCollideWithDodec", RpcTarget.All, viewIDOfObject);
        }
    }
}