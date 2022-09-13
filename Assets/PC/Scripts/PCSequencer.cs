using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Microsoft.MixedReality.Toolkit.UI;


public class PCSequencer : MonoBehaviour
{

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
    public float sequenceIntervalDelay = 2f;
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
        // Ensure Sequence is reset and only the first Sequence Item is visible...
        ResetSequence();

        OnPlay();
   

    }
  public  void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            OnPlay();
            print("space key was pressed");
        }
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
        if (CurrentIndex  < SequenceItems.Length )
        {
            // Debug.Log("what the index:  " + CurrentIndex);

            if (PhotonNetwork.IsMasterClient)
            {
                myPV.RPC("ResizeCube", RpcTarget.All, CurrentIndex);
            }
           //ResizeCube(CurrentIndex);
  
            if (PhotonView.Find(SequenceItems[CurrentIndex].GetComponent<PhotonView>().ViewID).gameObject.GetComponent<pcInteraction>().isActiveToPlay == true)
            {
                Debug.Log("PlayNOte:  " + CurrentIndex);
                SerialCommunication.sendNote(PhotonView.Find(SequenceItems[CurrentIndex].GetComponent<PhotonView>().ViewID).GetComponent<pcInteraction>().allCheckInsideSphere.currentNote);
                Debug.Log("sending note to arduino from sequencer");
              
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

    public void OnSliderUpdated(SliderEventData eventData)
    {
        float current = float.Parse($"{eventData.NewValue:F2}") ;
        myPV.RPC("SpeedAdjust", RpcTarget.All, current);

    }

    [PunRPC]
    void SpeedAdjust(float speed)
    {
        sequenceIntervalDelay = speed;

    }

    [PunRPC]
    void ResizeCube(int index)
    {
       
        {
            for (int i = 0; i < SequenceItems.Length; i++)
            {
                if (i == index)
                    SequenceItems[i].GetComponent<MeshFilter>().mesh = on;
                // PhotonView.Find(SequenceItems[i].GetComponent<PhotonView>().ViewID).gameObject.GetComponent<MeshFilter>().mesh = on;
                //  SequenceItems[i].gameObject.GetComponent<MeshFilter>().mesh = on;

                else
                    SequenceItems[i].GetComponent<MeshFilter>().mesh = off;
                //PhotonView.Find(SequenceItems[i].GetComponent<PhotonView>().ViewID).gameObject.GetComponent<MeshFilter>().mesh = off;
               // SequenceItems[i].gameObject.GetComponent<MeshFilter>().mesh = off;
                //PhotonView.Find(SequenceItems[i].GetComponent<PhotonView>().ViewID).gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
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