using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class SerialCommunication : MonoBehaviour
{

    // PUT here your port name 
    public static SerialPort sp = new SerialPort("COM5", 9600);

    // Start is called before the first frame update
    void Start()
    {
        OpenConnection();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            sendNote("X");
        }
    }


    public void OpenConnection()
    {
        if (sp != null)
        {
            if (sp.IsOpen)
            {
                sp.Close();
                print("Closing port, because it's already open");
            }
            else
            {
                sp.Open();
                sp.ReadTimeout = 100;
                print("port open");
            }
        }
        else
        {
            if (sp.IsOpen)
            {
                print("port is already open");
            }
            else
            {
                print("port == null");
            }
        }

    }

    void OnApplicationQuit()
    {
        sp.Close();
    }


    public static void sendNote(string note)
    {
        if (note != null)
        {
            sp.Write(note);
            sp.Write("X");
        //    Debug.Log("Note Sent To Arduino: " + note);
        }
    }

  public static IEnumerator func(string note)
    {
        sp.Write(note);
        yield return new WaitForSecondsRealtime(1); //Wait 1 second
        sp.Write("OFF");
    }


    public static void sendN()
    {
        sp.Write("OFF");
        Debug.Log("Pressed n");
    }
    public static void sendRed()
    {
        sp.Write("r");
    }

}
