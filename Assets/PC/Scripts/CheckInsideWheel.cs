using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


public class CheckInsideWheel : MonoBehaviour
{
    Collider m_Collider;
    public bool wheelBoxActive = false;
    public string activeNote;
    public GameObject wheelBox;
    
    private PhotonView myPV;
    private PhotonView myPVDodec;
    private int myPVInt;
    public GameObject dodec;

    public Material Green;
    public Material Red;
    public Material dodecColor;
    // Start is called before the first frame update
    void Start()
    {
       
        m_Collider = GetComponent<Collider>();
        m_Collider.enabled = true;
        myPV = GetComponent<PhotonView>();
        myPVInt = this.GetComponent<PhotonView>().ViewID;

    }

    private void Update()
    {
        if (dodec != null)
        {
            if (wheelBoxActive == true && dodec.GetComponent<SecCheck>().isActiveInSeq == true)
            {
                dodec.transform.position = transform.position;
                
            }
            else
            {
               
            }
        }
    }


    [PunRPC]
    public void RPC_OnWheelTrigger(int viewID)
    {
        Debug.Log("trigger wheel ");
        PhotonView.Find(viewID).gameObject.GetComponent<MeshRenderer>().material = Green;
        //send to remote call (on dodec) and trigger function Serial Communication
        
        if (myPVDodec.IsMine)
        {
            SerialCommunication.sendNote(activeNote);
          
        }
      
    }
        // Update is called once per frame
    void  OnTriggerEnter(Collider collision)
    {

     if (collision.gameObject.CompareTag("LWTrigger")  && wheelBoxActive == true)
        {
            int viewIDOfObject = collision.GetComponent<PhotonView>().ViewID;
            myPVDodec = dodec.GetComponent<PhotonView>();
            //calls the RPC on this script
            PhotonView.Find(myPVInt).RPC("RPC_OnWheelTrigger", RpcTarget.All, viewIDOfObject);
            Debug.Log("trigger wheel enter 2 ");
        }
        if (collision.gameObject.CompareTag("RWTrigger") && wheelBoxActive == true)
        {
            myPVDodec = dodec.GetComponent<PhotonView>();
            int viewIDOfObject = collision.GetComponent<PhotonView>().ViewID;

            //calls the RPC on this script
            PhotonView.Find(myPVInt).RPC("RPC_OnWheelTrigger", RpcTarget.All, viewIDOfObject);
          
        }
    }

    [PunRPC]
    public void RPC_OnWheelTriggerExit(int viewID)
    {
        //PhotonView.Find(myPVInt).gameObject.GetComponent<CheckInsideWheel>().wheelBox.GetComponent<MeshRenderer>().material.color = Color.green;
        PhotonView.Find(viewID).gameObject.GetComponent<MeshRenderer>().material = Red;
        //send to remote call (on dodec) and trigger function Serial Communication
        SerialCommunication.sendNote("X");
        Debug.Log("trigger wheel exit 2 ");
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("LWTrigger") && wheelBoxActive == true)
        {
            myPVDodec = null;
            int viewIDOfObject = collision.GetComponent<PhotonView>().ViewID;
        
            //calls the RPC on this script
            PhotonView.Find(myPVInt).RPC("RPC_OnWheelTriggerExit", RpcTarget.All, viewIDOfObject);
            
        }
        if (collision.gameObject.CompareTag("RWTrigger") && wheelBoxActive == true)
        {
            myPVDodec = null;

            int viewIDOfObject = collision.GetComponent<PhotonView>().ViewID;

            //calls the RPC on this script
            PhotonView.Find(myPVInt).RPC("RPC_OnWheelTriggerExit", RpcTarget.All, viewIDOfObject);
         
        }
    }

}
