using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine.Events;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;


public class PCSequencer : MonoBehaviour
{
    public UnityEvent BigExplosionEvent;
    [SerializeField]
    private TextMeshPro textMesh = null;
    public bool play = true;
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
    public float sequenceIntervalDelay = 0.2f;

    private int _currentIndex;
    /// <summary>
    /// The current sequence index.
    /// </summary>
    /// 

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
     
        ResetSequence();
       // myPV.RPC("OnPlay", RpcTarget.AllBuffered);

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
            textMesh.text = "PAUSE";
        }
        else
        {
            textMesh.text = "PLAY";
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
        // Reset sequence back to start...
        if (CurrentIndex == SequenceItems.Length - 1)
        {
            ResetSequence();
        }

 
        StartCoroutine("func");
     
      //  Debug.Log("Sequence Item " + CurrentIndex);
    }


    IEnumerator func()
    {
        while (true)
        {
            OnNext();
           
            yield return new WaitForSecondsRealtime(sequenceIntervalDelay); //Wait 1 second

        }
    }

    public void Update()
    {

       
    }

    private void OnNext()
    {
        if (play == true)
        {
            if (CurrentIndex < SequenceItems.Length)
            {
                
                    myPV.RPC("ResizeCube", RpcTarget.All, CurrentIndex);
                
                PhotonView currentIndexPV = SequenceItems[CurrentIndex].GetComponent<PhotonView>();
                if (currentIndexPV.GetComponent<pcInteraction>().isActiveToPlay == true)
                {
                    SerialCommunication.sendNote(currentIndexPV.GetComponent<pcInteraction>().allCheckInsideSphere.currentNote);
                
                }
                //else if (SequenceItems[CurrentIndex].GetComponent<pcInteraction>().isActiveToPlay == false)
                //{

                //}
               
                CurrentIndex++;
            }
            else
            {
                //  ResetSequence();
                myPV.RPC("ResetSequence", RpcTarget.All);

            }
        }
    }

    public void OnSliderUpdated(SliderEventData eventData)
    {
        float current = float.Parse($"{eventData.NewValue:F2}");
        myPV.RPC("SpeedAdjust", RpcTarget.All, current);

    }

  [PunRPC]
    void SpeedAdjust(float speed)
    {
        sequenceIntervalDelay = speed;
        textMesh.text = sequenceIntervalDelay.ToString();

    }

    [PunRPC]
    void ResizeCube(int index)
    {

            for (int i = 0; i < SequenceItems.Length; i++)
            {
                if (i == index)
                    SequenceItems[i].GetComponent<MeshFilter>().mesh = on;
               
                else
                    SequenceItems[i].GetComponent<MeshFilter>().mesh = off;
               
            }
        }
    

    /// <summary>
    /// Resests the sequence back to the beginning.
    /// </summary>
    [PunRPC]
    private void ResetSequence()
    {
        //foreach (var go in SequenceItems)
        //{

        //}
//#if UNITY_EDITOR
//        Debug.Log("reset seq");
//#endif
        CurrentIndex = 0;

    }
}