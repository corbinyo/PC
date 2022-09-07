using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wheeltrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public bool wheelBoxActive;
  
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collision)

    {
       
        if (collision.gameObject.CompareTag("LWTrigger") && wheelBoxActive)
        {
          //  SerialCommunication.sendNote();
        }
    }
}
