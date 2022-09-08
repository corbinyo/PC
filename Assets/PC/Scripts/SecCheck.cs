using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SecCheck : MonoBehaviour
{
   // public GameObject seqAvoid;
    private Rigidbody rb;
    public string note;
    public bool isActiveInSeq;
    private PhotonView myPV;
    private int myPVInt;
    private MeshRenderer myPVMeshRend;
    private SecCheck mySecCheck;
    public Material on;
    public Material off;
    public Material manip;
    //this script is called when the "Sec" is manipulated. It checks to see if it is currently in active in a Sequencer
    // Start is called before the first frame update
    void Start()
    {
        myPV = GetComponent<PhotonView>();
        myPVInt = this.GetComponent<PhotonView>().ViewID;
        mySecCheck = PhotonView.Find(myPVInt).gameObject.GetComponent<SecCheck>();
        rb = PhotonView.Find(myPVInt).gameObject.GetComponent<Rigidbody>();
        myPVMeshRend = PhotonView.Find(myPVInt).gameObject.GetComponent<MeshRenderer>();
      
    }

    [PunRPC]
    void RPC_startManipulate()
    {
       rb.constraints = RigidbodyConstraints.None;
       
    }

    public void startManipulate()
    {
        myPV.RPC("RPC_startManipulate", RpcTarget.All);
        Debug.Log("start manip");
    }

    [PunRPC]
    void RPC_endManipulate()
    {

        if (!mySecCheck.isActiveInSeq)
        {
            myPVMeshRend.material = off;
        }
    }
  

    public void endManipulate()
    {
        myPV.RPC("RPC_endManipulate", RpcTarget.All);
        Debug.Log("end manip");
    }


    [PunRPC]
     public  void RPC_OnTriggerEnterSequencer(int viewID)
    {
        Debug.Log("on enter sequencer box");

        rb.constraints = RigidbodyConstraints.FreezeAll;

        mySecCheck.isActiveInSeq = true;

        myPVMeshRend.GetComponent<MeshRenderer>().material = on;

        //Passing all information to  the sphere

        PhotonView.Find(viewID).gameObject.GetComponent<checkInside>().currentNote = note;

        PhotonView.Find(viewID).gameObject.GetComponent<checkInside>().sequencerBoxActive = true;

        PhotonView.Find(viewID).gameObject.GetComponent<MeshRenderer>().material = on;
        //calls the RPC on checkinside script
        PhotonView.Find(viewID).RPC("RPC_OnTriggerCollideWithDodec", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_OnTriggerEnterWheel(int viewID)
    {
        Debug.Log("on enter wheel box");

        rb.constraints = RigidbodyConstraints.FreezeAll;

        mySecCheck.isActiveInSeq = true;

        myPVMeshRend.GetComponent<MeshRenderer>().material = on;

        //Passing all information to  the sphere

        PhotonView.Find(viewID).gameObject.GetComponent<CheckInsideWheel>().activeNote = note;

        PhotonView.Find(viewID).gameObject.GetComponent<CheckInsideWheel>().wheelBoxActive = true;

        PhotonView.Find(viewID).gameObject.GetComponent<CheckInsideWheel>().wheelBox.GetComponent<MeshRenderer>().material = on;


        //Transform m_NewTransform = collision.gameObject.transform;

        PhotonView.Find(myPVInt).gameObject.transform.parent = PhotonView.Find(viewID).gameObject.transform;

        //this.transform.localPosition = new Vector3(0, 0, 0);
        //calls the RPC on checkinside script
        PhotonView.Find(viewID).RPC("RPC_OnTriggerCollideWithDodec", RpcTarget.All);
    }


    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("WheelNotationSphere"))
        {

            int viewIDOfObject = collision.GetComponent<PhotonView>().ViewID;

            PhotonView.Find(myPVInt).RPC("RPC_OnTriggerEnterWheel", RpcTarget.All, viewIDOfObject);

            Debug.Log("RPC_OnTriggerWheelFunction Called" + "Sequencer Added To Collider" + collision.gameObject.name);

        }

        if (collision.gameObject.CompareTag("SequencerNotationSphere"))
        {

            int viewIDOfObject = collision.GetComponent<PhotonView>().ViewID;

            //calls the RPC on this script
            PhotonView.Find(myPVInt).RPC("RPC_OnTriggerEnterSequencer", RpcTarget.All, viewIDOfObject);

            Debug.Log("RPC_OnTriggerSequencer Function Called" + "Sequencer Added To Collider" + collision.gameObject.name);

        }




    }
    [PunRPC]
    void RPC_OnTriggerExitSequencer(int viewID)
    {
        Debug.Log("on exit sequencer box");

        rb.constraints = RigidbodyConstraints.None;

        mySecCheck.isActiveInSeq = false;

        myPVMeshRend.GetComponent<MeshRenderer>().material = off;

        //Passing all information to  the sphere

        PhotonView.Find(viewID).gameObject.GetComponent<checkInside>().currentNote = null;

        PhotonView.Find(viewID).gameObject.GetComponent<checkInside>().sequencerBoxActive = false;

        PhotonView.Find(viewID).gameObject.GetComponent<MeshRenderer>().material = off;
        //calls the RPC on checkinside script
        PhotonView.Find(viewID).RPC("RPC_OnTriggerExitCollideWithDodec", RpcTarget.All);
    }

    void RPC_OnTriggerExitWheel(int viewID)
    {
        Debug.Log("on ecit wheel box");

        rb.constraints = RigidbodyConstraints.None;

        mySecCheck.isActiveInSeq = false;

        myPVMeshRend.GetComponent<MeshRenderer>().material = off;

        //Passing all information to  the sphere

        PhotonView.Find(viewID).gameObject.GetComponent<CheckInsideWheel>().activeNote = null;

        PhotonView.Find(viewID).gameObject.GetComponent<CheckInsideWheel>().wheelBoxActive = false;

        PhotonView.Find(viewID).gameObject.GetComponent<CheckInsideWheel>().wheelBox.GetComponent<MeshRenderer>().material = off;


        //Transform m_NewTransform = collision.gameObject.transform;

        PhotonView.Find(myPVInt).gameObject.transform.parent = null;

        //this.transform.localPosition = new Vector3(0, 0, 0);
        //calls the RPC on checkinside script
        PhotonView.Find(viewID).RPC("RPC_OnTriggerCollideWithDodec", RpcTarget.All);
    
}


    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("WheelNotationSphere"))
        {
            int viewIDOfObject = collision.GetComponent<PhotonView>().ViewID;
            //calls the RPC on this script
            PhotonView.Find(myPVInt).RPC("RPC_OnTriggerExitWheel", RpcTarget.All, viewIDOfObject);

            Debug.Log("RPC_OnTriggerExitWheel Function Called" + "Sequencer Removed From Collider" + collision.gameObject.name);



        }
        if (collision.gameObject.CompareTag("SequencerNotationSphere"))
        {
            int viewIDOfObject = collision.GetComponent<PhotonView>().ViewID;

            //calls the RPC on this script
            PhotonView.Find(myPVInt).RPC("RPC_OnTriggerExitSequencer", RpcTarget.All, viewIDOfObject);

            Debug.Log("RPC_OnTriggerExitSequencer Function Called" + "Sequencer Removed From Collider" + collision.gameObject.name);


        }
    }
}
