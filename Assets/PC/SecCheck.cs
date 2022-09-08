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
        //Physics.IgnoreCollision(seqAvoid.GetComponent<Collider>(), GetComponent<Collider>());
        rb = PhotonView.Find(myPVInt).gameObject.GetComponent<Rigidbody>();
        myPVMeshRend = PhotonView.Find(myPVInt).gameObject.GetComponent<MeshRenderer>();
      
    }

    [PunRPC]
    void RPC_startManipulate()
    {

       rb.constraints = RigidbodyConstraints.None;
        //myPVMeshRend.material = manip;
    }

    [PunRPC]
    void RPC_endManipulate()
    {

        if (!mySecCheck.isActiveInSeq)
        {
            myPVMeshRend.material = off;
        }
    }
    public void startManipulate()
    {
        myPV.RPC("RPC_startManipulate", RpcTarget.All);
       Debug.Log("start manip");
      
      

    }

    public void endManipulate()
    {
        myPV.RPC("RPC_endManipulate", RpcTarget.All);
        Debug.Log("end manip");
        //rb.constraints = RigidbodyConstraints.None;
        //if (!isActiveInSeq)
        //{
        //    this.GetComponent<MeshRenderer>().material.color = Color.red;
        //}
    }


    [PunRPC]
    void RPC_OnTriggerEnterSequencer(int viewID)
    {
        Debug.Log("on enter seq box");

        rb.constraints = RigidbodyConstraints.FreezeAll;
      
        PhotonView.Find(viewID).gameObject.GetComponent<checkInside>().currentNote = note;

        mySecCheck.isActiveInSeq = true;

        PhotonView.Find(viewID).gameObject.GetComponent<checkInside>().sequencerBoxActive = true;

        Transform m_NewTransform = PhotonView.Find(viewID).gameObject.transform;

      //  PhotonView.Find(myPVInt).transform.parent = m_NewTransform;

       // this.transform.localPosition = new Vector3(0, 0, 0);
       
        this.GetComponent<MeshRenderer>().material = on;
    }

    [PunRPC]
    void RPC_OnTriggerExitSequencer(int viewID)
    {
        rb.constraints = RigidbodyConstraints.None;
        // Debug.Log("Sequencer Removed From Collider" + collision.gameObject.name);
        PhotonView.Find(viewID).gameObject.GetComponent<checkInside>().currentNote = null;
        mySecCheck.isActiveInSeq = false;



        //collision.GetComponent<checkInside>().currentNote = null;
        //isActiveInSeq = false;
        //CheckInsideWheel active = collision.gameObject.GetComponent<CheckInsideWheel>();
        PhotonView.Find(myPVInt).transform.parent = null;
       
       myPVMeshRend.GetComponent<MeshRenderer>().material = off;
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("WheelNotationSphere"))
        {
          
            isActiveInSeq = true;

            collision.GetComponent<CheckInsideWheel>().wheelBoxActive = true;

            collision.GetComponent<CheckInsideWheel>().activeNote = note;


            collision.GetComponent<CheckInsideWheel>().wheelBox.GetComponent<MeshRenderer>().material.color = Color.green;

            // collision.GetComponent<SphereCollider>().radius = 0f;

            Transform m_NewTransform = collision.gameObject.transform;
            
            this.transform.parent = m_NewTransform;
            
            this.transform.localPosition = new Vector3(0, 0, 0);
            
            rb.constraints = RigidbodyConstraints.FreezeAll;

            this.GetComponent<MeshRenderer>().material = on;



        }

        if (collision.gameObject.CompareTag("SequencerNotationSphere"))
        {
        
         int viewIDOfObject = collision.GetComponent<PhotonView>().ViewID;

            this.myPV.RPC("RPC_OnTriggerEnterSequencer", RpcTarget.All, viewIDOfObject);

            Debug.Log("RPC_OnTriggerSequencer Function Called" + "Sequencer Added To Collider" + collision.gameObject.name);

            //isActiveInSeq = true;

            //collision.GetComponent<checkInside>().currentNote = note;

            //collision.GetComponent<checkInside>().sequencerBoxActive = true;

            //Transform m_NewTransform = collision.gameObject.transform;
           
            //this.transform.parent = m_NewTransform;
            
            //this.transform.localPosition = new Vector3(0, 0, 0);
            
            //rb.constraints = RigidbodyConstraints.FreezeAll;

            //this.GetComponent<MeshRenderer>().material = on;
        }




    }


            void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("WheelNotationSphere"))
        {
            Debug.Log("WHEEL DE COLLISION");


      
            isActiveInSeq = false;
            CheckInsideWheel active = collision.gameObject.GetComponent<CheckInsideWheel>();

            collision.GetComponent<CheckInsideWheel>().wheelBox.GetComponent<MeshRenderer>().material.color = Color.red;
            // collision.GetComponent<SphereCollider>().radius = 0.5f;
            transform.parent = null;
            active.wheelBoxActive = false;

            rb.constraints = RigidbodyConstraints.None;

            this.GetComponent<MeshRenderer>().material = off;
          


        }
        if (collision.gameObject.CompareTag("SequencerNotationSphere"))
        {
            int viewIDOfObject = collision.GetComponent<PhotonView>().ViewID;
            Debug.Log("Sequencer Removed From Collider" + collision.gameObject.name);
            this.myPV.RPC("RPC_OnTriggerExitSequencer", RpcTarget.All, viewIDOfObject);
            //collision.GetComponent<checkInside>().currentNote = null;
            //isActiveInSeq = false;
            //CheckInsideWheel active = collision.gameObject.GetComponent<CheckInsideWheel>();
            //transform.parent = null;
            //rb.constraints = RigidbodyConstraints.None;
            //this.GetComponent<MeshRenderer>().material = off;

        }
    }
}
