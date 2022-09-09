using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


public class CheckInsideWheel : MonoBehaviour
{
    Collider m_Collider;
    public bool wheelBoxActive;
    public string activeNote;
    public GameObject wheelBox;
    public RemoteCall remote;
    private PhotonView myPV;
    private int myPVInt;
    // Start is called before the first frame update
    void Start()
    {
       
        m_Collider = GetComponent<Collider>();
        m_Collider.enabled = true;
        myPV = GetComponent<PhotonView>();
        myPVInt = this.GetComponent<PhotonView>().ViewID;
    }

    [PunRPC]
    public void RPC_OnWheelTrigger(int viewID)
    {
        PhotonView.Find(myPVInt).gameObject.GetComponent<CheckInsideWheel>().wheelBox.GetComponent<MeshRenderer>().material.color = Color.yellow; 
        PhotonView.Find(viewID).gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
        //send to remote call (on dodec) and trigger function Serial Communication
        SerialCommunication.sendNote(activeNote);
    }
        // Update is called once per frame
        void  OnTriggerEnter(Collider collision)
    {
     if (collision.gameObject.CompareTag("LWTrigger") && wheelBoxActive == true)
        {
            int viewIDOfObject = collision.GetComponent<PhotonView>().ViewID;
 
            //calls the RPC on this script
            PhotonView.Find(myPVInt).RPC("RPC_OnWheelTrigger", RpcTarget.All, viewIDOfObject);
        }
    }

    [PunRPC]
    public void RPC_OnWheelTriggerExit(int viewID)
    {
        PhotonView.Find(myPVInt).gameObject.GetComponent<CheckInsideWheel>().wheelBox.GetComponent<MeshRenderer>().material.color = Color.green;
        PhotonView.Find(viewID).gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        //send to remote call (on dodec) and trigger function Serial Communication
        SerialCommunication.sendNote("X");
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("LWTrigger") && wheelBoxActive == true)
        {
            int viewIDOfObject = collision.GetComponent<PhotonView>().ViewID;

            //calls the RPC on this script
            PhotonView.Find(myPVInt).RPC("RPC_OnWheelTriggerExit", RpcTarget.All, viewIDOfObject);
        }
    }

}
