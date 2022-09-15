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
    public RotateAxis rotate;
    public PCSequencer pcSequencer;
    private OnOff onOff;
    void Start()
    {
        myPV = this.GetComponent<PhotonView>();
        mytext = myPV.GetComponent<OnOff>().mytext;
        onOff = myPV.GetComponent<OnOff>().onOff;
    }

  
    public void PlayPauseWheel()
    {
        myPV.RPC("PauseWheel_RPC", RpcTarget.All);
    }

    public void PlayPauseSequencer()
    {
        myPV.RPC("PauseSeq_RPC", RpcTarget.All);
    }

    [PunRPC]
    public void PauseSeq_RPC()
    {
        counter++;
        if (counter % 2 == 1)
        {
            Debug.Log("PAUSE");
            mytext.text = "PLAY";
            
                pcSequencer.play = false;
            
           
        }
        else
        {
            Debug.Log("PAUSE");
            mytext.text = "PAUSE";

            pcSequencer.play = true;
            
        }
    }

    [PunRPC]
    public void PauseWheel_RPC()
    {
        counter++;
        if (counter % 2 == 1)
        {
            Debug.Log("PAUSE");
            mytext.text = "PLAY";


            autoRotate.play = false;
            

        }
        else
        {
            Debug.Log("PLAY");
            mytext.text = "PAUSE";

            autoRotate.play = true;
            
        

        }
    }
}