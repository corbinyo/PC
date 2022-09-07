using System.Collections;
using System.Collections.Generic;
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
    void Start()
    {
      
        m_Collider = GetComponent<Collider>();
       
    }

    public void SphereColliderOn()
    {
        m_Collider.enabled = true;
    }

    void OnTriggerEnter(Collider collision)

    {

        if (collision.gameObject.tag == "sec")
        {
            Debug.Log("make box active");
            pc.setToPlay();
        }
    }

    void OnTriggerExit(Collider collision)

    {

        if (collision.gameObject.tag == "sec")
        {

            Debug.Log("make box inactive");
            pc.setToStop();
        }
    }
}