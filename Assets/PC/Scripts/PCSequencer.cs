using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using UnityEngine.Events;
using TMPro;



public class PCSequencer : MonoBehaviour
{
    public UnityEvent BigExplosionEvent;
    public bool play = true;
    public TMPro.TextMeshPro text;
  
    [Header("Sequence Controller")]
    private PhotonView myPV;
    public Mesh on;
    public Mesh off;
    [Tooltip("Drag in a bunch of GameObject you want in the sequence (must be in correct order before dragging in).")]
    /// <summary>
    /// List of GameObjects in sequence (must be in correct order)
    /// </summary>
    public GameObject[] SequenceItems;
    [Tooltip("The amount of time in seconds between sequence items.")]
    /// <summary>
    /// The interval of time in seconds between sequences.
    /// </summary>
    public float sequenceIntervalDelay = 0f;
    private int _currentIndex;
    /// <summary>
    /// The current sequence index.
    /// </summary>
    public int CurrentIndex
     

    {
        get { return _currentIndex; }
        set
        {
            if (value != _currentIndex)
            {
                // Ensure current index isn't more/less than the index bounds of the given Sequence Items...
                if (_currentIndex >= 0 || _currentIndex <= SequenceItems.Length - 1)
                {
                   // Debug.Log("is this a thing?:   " + value);
                    _currentIndex = value;

                }
            }
        }
    }
    /// <summary>
    /// Get things ready before first update occurs.
    /// </summary>
    void Start()
    {
        myPV = GetComponent<PhotonView>();
        if (BigExplosionEvent == null)
            BigExplosionEvent = new UnityEvent();

        var targetInfo = UnityEvent.GetValidMethodInfo(this, nameof(ExplodeMe), new Type[0]);
        UnityAction methodDelegate = Delegate.CreateDelegate(typeof(UnityAction), this, targetInfo) as UnityAction;
        //UnityEventTools.AddPersistentListener(BigExplosionEvent, methodDelegate);
        // Ensure Sequence is reset and only the first Sequence Item is visible...
        ResetSequence();
        OnPlay();
 
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


    /// <summary>
    /// On play button pressed.
    /// </summary>
    private void OnPlay()
    {
        Debug.Log("on play");
        // Reset sequence back to start...
        if (CurrentIndex == SequenceItems.Length - 1)
        {
            ResetSequence();
        }

        // When Play button press, repeat call the OnNext() method...
        StartCoroutine("func");
        // InvokeRepeating("OnNext", sequenceIntervalDelay, sequenceIntervalDelay);
        Debug.Log("Sequence Item " + CurrentIndex);
    }


    IEnumerator func()
    {
        while (true)
        {
            OnNext();
           
            yield return new WaitForSecondsRealtime(sequenceIntervalDelay); //Wait 1 second
            
        }
    }

    private void OnNext()
    {
       if (play)
        {
            if (CurrentIndex < SequenceItems.Length)
            {
                // Debug.Log("what the index:  " + CurrentIndex);

                if (myPV.IsMine)
                {
                    myPV.RPC("ResizeCube", RpcTarget.AllBuffered, CurrentIndex);
                 }


                if (PhotonView.Find(SequenceItems[CurrentIndex].GetComponent<PhotonView>().ViewID).gameObject.GetComponent<pcInteraction>().isActiveToPlay == true)
                {
                   // Debug.Log("PlayNOte:  " + CurrentIndex);
                    SerialCommunication.sendNote(PhotonView.Find(SequenceItems[CurrentIndex].GetComponent<PhotonView>().ViewID).GetComponent<pcInteraction>().allCheckInsideSphere.currentNote);
                   // Debug.Log("sending note to arduino from sequencer");

                }
                else if (SequenceItems[CurrentIndex].GetComponent<pcInteraction>().isActiveToPlay == false)
                {

                }
                CurrentIndex++;
            }
            else
            {
                ResetSequence();
            }
        }
    }
    [PunRPC]
   void SeqSpeed_RPC(float updatedSpeed)
    {
        sequenceIntervalDelay = updatedSpeed;
    }


    public void OnSliderUpdated(SliderEventData eventData)
    {

        myPV.RPC("SeqSpeed_RPC", RpcTarget.AllBuffered, float.Parse($"{eventData.NewValue:F2}"));

    }

    [PunRPC]
    void ResizeCube(int index)
    {
        {
            for (int i = 0; i < SequenceItems.Length; i++)
            {
                if (i == index)
                    SequenceItems[i].GetComponent<MeshFilter>().mesh = on;
                else
                    SequenceItems[i].GetComponent<MeshFilter>().mesh = off;
                
            }
        }
    }

    /// <summary>
    /// Resests the sequence back to the beginning.
    /// </summary>
    private void ResetSequence()
    {
        foreach (var go in SequenceItems)
        {
            // go.SetActive(false);
          //  go.GetComponent<MeshRenderer>().material.color = Color.white;
        }
       // SequenceItems.First().SetActive(true);
        CurrentIndex = 0;
    }
}