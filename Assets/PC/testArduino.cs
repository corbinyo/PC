using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testArduino : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Ge()
    {
        Debug.Log("YO");
      //  SerialCommunication.sendY();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("up"))
        {
            Debug.Log("YO");
           // SerialCommunication.sendY();
        }
        
    }
}
