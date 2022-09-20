using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using System;
using UnityEngine.Events;
using TMPro;
public class RotateAxis : MonoBehaviour
{
    //Rotational Speed
    public float speed = 0f;

    //Forward Direction
    public bool ForwardX = true;
    public bool ForwardY = false;
    public bool ForwardZ = false;

    //Reverse Direction
    public bool ReverseX = false;
    public bool ReverseY = false;
    public bool ReverseZ = false;

    public bool play = true;

    private PhotonView myPV;
    public UnityEvent BigExplosionEvent;
    public TMPro.TextMeshPro text;
    [SerializeField]
    private TextMeshPro textMeshSpeed = null;
    void Start()
    {
        myPV = this.GetComponent<PhotonView>();
        if (BigExplosionEvent == null)
            BigExplosionEvent = new UnityEvent();

        var targetInfo = UnityEvent.GetValidMethodInfo(this, nameof(ExplodeMe), new Type[0]);
        UnityAction methodDelegate = Delegate.CreateDelegate(typeof(UnityAction), this, targetInfo) as UnityAction;
        //UnityEventTools.AddPersistentListener(BigExplosionEvent, methodDelegate);
    }
    private  void FixedUpdate()
    {
        Rotate();
    }

    [PunRPC]
    public void PlayPause_RPC()
    {
        play = !play;
        if (play)
        {
            text.text = "PAUSE";
        }
        else
        {
            text.text = "PLAY";
        }
    }


    public void ExplodeMe()
    {
        myPV.RPC("PlayPause_RPC", RpcTarget.All);
        
    }

    [PunRPC]
    public void WheelSpeed_RPC(float updatedSpeed)
    {
        //Debug.Log("Slider Updated");
        speed = updatedSpeed;
        textMeshSpeed.text = speed.ToString();
    }


        public void OnSliderUpdated(SliderEventData eventData)
    {
        
        myPV.RPC("WheelSpeed_RPC", RpcTarget.All, float.Parse($"{eventData.NewValue:F2}") * 100f);

    }

    void Rotate()
    {
        //Forward Direction
        if (ForwardX == true && play == true)
        {
            transform.Rotate(Time.deltaTime * speed, 0, 0);
        }
        if (ForwardY == true && play == true)
        {
            transform.Rotate(0, Time.deltaTime * speed, 0, Space.Self);
        }
        if (ForwardZ == true && play == true)
        {
            transform.Rotate(0, 0, Time.deltaTime * speed, Space.Self);
        }
        //Reverse Direction
        if (ReverseX == true && play == true)
        {
            transform.Rotate(-Time.deltaTime * speed, 0, 0, Space.Self);
        }
        if (ReverseY == true && play == true)
        {
            transform.Rotate(0, -Time.deltaTime * speed, 0, Space.Self);
        }
        if (ReverseZ == true && play == true)
        {
            transform.Rotate(0, 0, -Time.deltaTime * speed, Space.Self);
        }
    }
}