using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class AutoRotate : MonoBehaviour
{
    public float Speed;

    public bool RotateOnX = true;
    public bool RotateOnY = true;
    public bool RotateOnZ = true;


    public void OnSliderUpdated(SliderEventData eventData)
    {
       
           Speed = float.Parse($"{eventData.NewValue:F2}") * 100;
        
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