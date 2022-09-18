using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
public class SliderControl : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro textMesh = null;
    private PhotonView myPV;

    void Start()
    {
        myPV = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void ChangeSliderValue_RPC(float speedVisual)
    {
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMeshPro>();
        }

        if (textMesh != null)
        {
            textMesh.text = speedVisual.ToString();
        }
    }

    public void OnSliderUpdated(SliderEventData eventData)
    {
            myPV.RPC("PlayPause_RPC", RpcTarget.Others, float.Parse($"{eventData.NewValue:F2}"));
        }
}

