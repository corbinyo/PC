// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.Serialization;
using Photon.Pun;
using Photon.Realtime;
namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    [AddComponentMenu("Scripts/MRTK/Examples/HandInteractionTouchRotate")]
    public class HandInteractionTouchRotate : HandInteractionTouch, IMixedRealityTouchHandler
    {
        [SerializeField]
        [FormerlySerializedAs("TargetObjectTransform")]
        private Transform targetObjectTransform = null;
        private MeshRenderer myPVMeshRend;
        public Material on;
        public Material off;
        public string note;
        private PhotonView myPV;
        private int myPVInt;
        [SerializeField]
        private float rotateSpeed = 300.0f;

        void Start()
        {
            myPV = GetComponent<PhotonView>();
            myPVInt = this.GetComponent<PhotonView>().ViewID;
        
            myPVMeshRend = PhotonView.Find(myPVInt).gameObject.GetComponentInChildren<MeshRenderer>();
        }

        [PunRPC]
        public void RPC_OnTriggerDrumPad(int viewID)
        {
           
            myPVMeshRend.GetComponent<MeshRenderer>().material = on;
            SerialCommunication.sendNote(note);
        }

        [PunRPC]
        public void RPC_OnStopDrumPad(int viewID)
        {

            myPVMeshRend.GetComponent<MeshRenderer>().material = off;
            //SerialCommunication.sendNote(note);
        }

        void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
        {
            if (targetObjectTransform != null)
            {
                PhotonView.Find(myPVInt).RPC("RPC_OnTriggerDrumPad", RpcTarget.All, myPVInt);
            }
        }



        void IMixedRealityTouchHandler.OnTouchCompleted(HandTrackingInputEventData eventData) 
        {
            if (targetObjectTransform != null)
            {

                PhotonView.Find(myPVInt).RPC("RPC_OnStopDrumPad", RpcTarget.All, myPVInt);
            }
        }


        void IMixedRealityTouchHandler.OnTouchUpdated(HandTrackingInputEventData eventData)
        {
            if (targetObjectTransform != null)
            {
                targetObjectTransform.Rotate(Vector3.up * (rotateSpeed * Time.deltaTime));
               
            }
        }
    }
}