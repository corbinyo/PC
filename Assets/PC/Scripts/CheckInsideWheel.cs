using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckInsideWheel : MonoBehaviour
{
    Collider m_Collider;
    public bool wheelBoxActive;
    public string activeNote;
    public GameObject wheelBox;
    public RemoteCall remote;
    // Start is called before the first frame update
    void Start()
    {
       
        m_Collider = GetComponent<Collider>();
        m_Collider.enabled = true;
    }


    // Update is called once per frame
    void  OnTriggerEnter(Collider collision)

    {

     
        if (collision.gameObject.CompareTag("sec"))
        {
            Debug.Log("Collided With Dodecahedron: " + collision.name);
            remote = collision.gameObject.GetComponent<RemoteCall>();

        }

        if (collision.gameObject.CompareTag("LWTrigger") && wheelBoxActive == true)
        {
            m_Collider.enabled = true;
            Debug.Log("Trigger COLLISION");
            collision.GetComponent<MeshRenderer>().material.color = Color.blue;
            wheelBox.GetComponent<MeshRenderer>().material.color = Color.blue;
            //send to remote call (on dodec) and trigger function Serial Communication
            remote.SoundYes(activeNote);
            //SerialCommunication.sendNote(activeNote);
        }
    }

    void OnTriggerExit(Collider collision)

    {

      

        if (collision.gameObject.CompareTag("LWTrigger") && wheelBoxActive == true)
        {
            // m_Collider.enabled = true;
            // Debug.Log("Trigger COLLISION");
            collision.GetComponent<MeshRenderer>().material.color = Color.green;
            wheelBox.GetComponent<MeshRenderer>().material.color = Color.green;
           // SerialCommunication.sendNote(activeNote);
        }
    }

}
