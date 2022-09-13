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
    void Start()
    {
        myPV = GetComponent<PhotonView>();
     


    }
    [PunRPC]
    void SpeedAdjust(float speed)
    {
        Speed = speed;

    }

    public void OnSliderUpdated(SliderEventData eventData)
    {
        float current = float.Parse($"{eventData.NewValue:F2}") * 100;
        myPV.RPC("SpeedAdjust", RpcTarget.All, current);
       
        
    }


void Update()
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