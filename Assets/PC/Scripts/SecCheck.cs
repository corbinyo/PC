using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

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
    public GameObject avoid;

    //this script is called when the "Sec" is manipulated. It checks to see if it is currently in active in a Sequencer
    // Start is called before the first frame update
    void Start()
    {
        myPV = GetComponent<PhotonView>();
        myPVInt = this.GetComponent<PhotonView>().ViewID;
        mySecCheck = myPV.gameObject.GetComponent<SecCheck>();
        rb = PhotonView.Find(myPVInt).gameObject.GetComponent<Rigidbody>();
        myPVMeshRend = myPV.gameObject.GetComponent<MeshRenderer>();
       // avoid = GameObject.FindWithTag("LWTrigger");

        if (avoid != null)
        {
            Physics.IgnoreCollision(avoid.GetComponent<Collider>(), myPV.gameObject.GetComponent<Collider>(), true);
        }
        else
        {
            Debug.Log("coulnd't find objects to avoid");
        }
    }
   

        [PunRPC]
    void RPC_startManipulate()
    {
        isActiveInSeq = false;
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    public void startManipulate()
    {
        myPV.RPC("RPC_startManipulate", RpcTarget.All);
        Debug.Log("start manip");
    }

    [PunRPC]
    void RPC_endManipulate()
    {
        //if (mySecCheck != null)
        //{
        //    if (mySecCheck.isActiveInSeq == false)
        //    {
        //        myPVMeshRend.material = off;
        //    }
        //}
    }
  

    public void endManipulate()
    {
        myPV.RPC("RPC_endManipulate", RpcTarget.All);
        Debug.Log("end manip");
    }

    [PunRPC]
    public void RPC_OnTriggerEnterSequencer(int viewID)
    {
            PhotonView pv = PhotonView.Find(viewID);

            rb.constraints = RigidbodyConstraints.FreezeAll;

            mySecCheck.isActiveInSeq = true;

            myPV.GetComponent<MeshRenderer>().material = on;

            //Passing all information to  the sphere

            pv.gameObject.GetComponent<checkInside>().currentNote = note;

            pv.gameObject.GetComponent<checkInside>().sequencerBoxActive = true;

            pv.gameObject.GetComponent<MeshRenderer>().material = on;

            // myPV.gameObject.GetComponent<ObjectManipulator>().ForceEndManipulation();
           
            Debug.Log("on enter sequencer box RPC SEC CHECK");
            //calls the RPC on checkinside script
            pv.RPC("RPC_OnTriggerCollideWithDodec", RpcTarget.All);
        
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
        
        PhotonView.Find(viewID).gameObject.GetComponent<CheckInsideWheel>().dodec = this.gameObject;

        myPV.gameObject.GetComponent<ObjectManipulator>().ForceEndManipulation();

        // PhotonView.Find(myPVInt).gameObject.transform.SetParent(PhotonView.Find(viewID).gameObject.transform);

        //calls the RPC on checkinsidewheel script
        // PhotonView.Find(viewID).RPC("RPC_OnTriggerCollideWithDodec", RpcTarget.All);


    }
  

    void OnTriggerEnter(Collider collision)
    {
        int viewIDOfObject = collision.GetComponent<PhotonView>().ViewID;

        if (collision.gameObject.CompareTag("WheelNotationSphere"))
        {
          
            //calls the RPC on this script
           myPV.RPC("RPC_OnTriggerEnterWheel", RpcTarget.All, viewIDOfObject);
          Debug.Log("RPC_OnTriggerWheelFunction Called" + "Sequencer Added To Collider" + collision.gameObject.name);
        }

        if (collision.gameObject.CompareTag("SequencerNotationSphere"))
        {
            Debug.Log("DODEC Entered SEQ");
           
            //calls the RPC on this script
            myPV.RPC("RPC_OnTriggerEnterSequencer", RpcTarget.All, viewIDOfObject);
            Debug.Log("RPC_OnTriggerSequencer Function Called" + "Sequencer Added To Collider" + collision.gameObject.name);

        }
    }

    [PunRPC]
    void RPC_OnTriggerExitSequencer(int viewID)
    {
       
            Debug.Log("on exit sequencer box");
            PhotonView pv = PhotonView.Find(viewID);
            rb.constraints = RigidbodyConstraints.None;

            mySecCheck.isActiveInSeq = false;

            myPV.GetComponent<MeshRenderer>().material = off;

            //Passing all information to  the sphere

            pv.GetComponent<checkInside>().currentNote = null;

            pv.GetComponent<checkInside>().sequencerBoxActive = false;

            pv.GetComponent<MeshRenderer>().material = off;
            //calls the RPC on checkinside script
            pv.RPC("RPC_OnTriggerExitCollideWithDodec", RpcTarget.All);
        }
    

    [PunRPC]
    void RPC_OnTriggerExitWheel(int viewID)
    {
      
            Debug.Log("on exit wheel box");

            rb.constraints = RigidbodyConstraints.None;

            mySecCheck.isActiveInSeq = false;

            myPVMeshRend.GetComponent<MeshRenderer>().material = off;

            //Passing all information to  the sphere

            PhotonView.Find(viewID).gameObject.GetComponent<CheckInsideWheel>().activeNote = null;

            PhotonView.Find(viewID).gameObject.GetComponent<CheckInsideWheel>().wheelBoxActive = false;

            PhotonView.Find(viewID).gameObject.GetComponent<CheckInsideWheel>().wheelBox.GetComponent<MeshRenderer>().material = off;

        
        //Transform m_NewTransform = collision.gameObject.transform;

     //  PhotonView.Find(myPVInt).gameObject.transform.parent = null;

        //this.transform.localPosition = new Vector3(0, 0, 0);
        //calls the RPC on checkinside script
       // PhotonView.Find(viewID).RPC("RPC_OnWheelTriggerExit", RpcTarget.All);
    
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
