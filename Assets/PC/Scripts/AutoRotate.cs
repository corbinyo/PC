using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Photon.Realtime;
public class AutoRotate : MonoBehaviour
{
    public float Speed;
    public bool RotateOnX = true;
    public bool RotateOnY = true;
    public bool RotateOnZ = true;
    private PhotonView myPV;
    public PinchSlider pinchSlider;
    public bool play = true;
    void Start()
    {
       myPV = GetComponent<PhotonView>();
       
      pinchSlider = myPV.GetComponent<AutoRotate>().pinchSlider;


    }
    [PunRPC]
    void SpeedAdjust(float speed)
    {
        Speed = speed;
        //Debug.Log("the speed is : " + speed);
        if (!myPV.IsMine)
        {
            pinchSlider.SliderValue = speed;
        }

    }

    public void OnSliderUpdated(SliderEventData eventData)
    {

        Speed = float.Parse($"{eventData.NewValue:F2}") * 100f;


       myPV.RPC("SpeedAdjust", RpcTarget.All, Speed);
       
        
    }


    void Update()
    {

        if (play == true)
        {
            Vector3 rotFactor = Vector3.one * Speed;

            if (!RotateOnX) rotFactor.x = 0;
            if (!RotateOnY) rotFactor.y = 0;
            if (!RotateOnZ) rotFactor.z = 0;

            transform.Rotate(
                rotFactor * Time.deltaTime
           );
        }
    }
}