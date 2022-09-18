using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;


public class Reset : MonoBehaviour
{
    private PhotonView myPV;
    public PinchSlider[] sliders;
    public GridObjectCollection[] gridObjects;

    void Start()
    {
        myPV = GetComponent<PhotonView>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            myPV.RPC("Restart", RpcTarget.All);
        }
    }
    [PunRPC]
    void Restart()
    {
        foreach (PinchSlider pinch in sliders)
        {
            pinch.SliderValue = 0.5f;
        }
        foreach (GridObjectCollection grids in gridObjects)
        {
            grids.UpdateCollection();
        }

    }
}