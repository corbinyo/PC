using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Photon.Realtime;
public class OnOff : MonoBehaviour
{

    public TMPro.TMP_Text mytext;

    public int counter = 0;
    private PhotonView myPV;
    public AutoRotate autoRotate;
    public PCSequencer pcSequencer;
    void Start()
    {
        myPV = this.GetComponent<PhotonView>();
        mytext = myPV.GetComponent<OnOff>().mytext;
    }


    public void PlayPause()
    {
        myPV.RPC("changeText_RPC", RpcTarget.All);
    }

    [PunRPC]
    public void changeText_RPC()
    {
        counter++;
        if (counter % 2 == 1)
        {
            Debug.Log("Pause");
            mytext.text = "PLAY";
            if (autoRotate != null)
            {
                autoRotate.play = false;
            }
            if (pcSequencer != null)
            {
              pcSequencer.play = false;
            }
           
        }
        else
        {
            Debug.Log("Play");
            mytext.text = "PAUSE";
            if (autoRotate != null)
            {
                autoRotate.play = true;
            }
            if (pcSequencer != null)
            {
                pcSequencer.play = true;
            }
            mytext.text = "Start";
            
        }
    }
}