using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;



public class PCSequencer : MonoBehaviour
{

    [Header("Sequence Controller")]

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

    private Button _playBtn;
    private Button _pauseBtn;
    private Button _prevBtn;
    private Button _nextBtn;


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
      
        // Ensure Sequence is reset and only the first Sequence Item is visible...
        ResetSequence();
        OnPlay();
        // AddListeners to UI Controls...
        // SubscribeToUIControls();

        // At start ensure Pause Button is hidden...
        // _pauseBtn.gameObject.SetActive(false);

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
    /// Get the UI controls from transform and add listeners.
    /// </summary>
    private void SubscribeToUIControls()
    {
        // Get Button components from children on this transform (NOTE: child names MUST be the same as what you are showing in Unity Editor)...
        _playBtn = transform.Find("btn_play").GetComponent<Button>();
        _pauseBtn = transform.Find("btn_pause").GetComponent<Button>();
        _prevBtn = transform.Find("btn_prev").GetComponent<Button>();
        _nextBtn = transform.Find("btn_next").GetComponent<Button>();

        // Add OnClick Listeners...
        _playBtn.onClick.AddListener(OnPlay);
        _pauseBtn.onClick.AddListener(OnPause);
        _prevBtn.onClick.AddListener(OnPrev);
        _nextBtn.onClick.AddListener(OnNext);
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

     
        // When Play button press, repeat call the OnNext() method..
        InvokeRepeating("OnNext", sequenceIntervalDelay, sequenceIntervalDelay);
        Debug.Log("Sequence Item " + CurrentIndex);
    }

    /// <summary>
    /// On pause button pressed.
    /// </summary>
    private void OnPause()
    {
        // Stop the repeated call to OnNext()...
        CancelInvoke("OnNext");

        // Flip play/pause button visibility...
        _playBtn.gameObject.SetActive(true);
        _pauseBtn.gameObject.SetActive(false);
    }

    /// <summary>
    /// On Prev Button Pressed.
    /// </summary>
    private void OnPrev()
    {
        if (CurrentIndex > 0)
        {
            // Hide the Sequence Item current index and decrement the index...
            SequenceItems[_currentIndex].SetActive(false);
            CurrentIndex--;
        }
    }

    /// <summary>
    /// On next Button pressed.
    /// </summary>


  
    private void OnNext()
    {
        if (CurrentIndex  < SequenceItems.Length )
        {
          // Debug.Log("what the index:  " + CurrentIndex);
            // If we are not at the end of the Sequence, increment the current index and make the Sequence Item at the new current index visible...
         
            //SequenceItems[CurrentIndex].SetActive(true);
            ResizeCube(CurrentIndex);
  
            if (PhotonView.Find(SequenceItems[CurrentIndex].GetComponent<PhotonView>().ViewID).gameObject.GetComponent<pcInteraction>().isActiveToPlay == true)
            {
                Debug.Log("PlayNOte:  " + CurrentIndex);
                SerialCommunication.sendNote(PhotonView.Find(SequenceItems[CurrentIndex].GetComponent<PhotonView>().ViewID).GetComponent<pcInteraction>().checkInsideSphere.currentNote);
                Debug.Log("sending note to arduino from sequencer");
                // SerialCommunication.sendNote("X");
            }
            else if (SequenceItems[CurrentIndex].GetComponent<pcInteraction>().isActiveToPlay == false)
            {
                //SerialCommunication.sendNote("X");
            }
            CurrentIndex++;
        }
        else
        {
            // If we are at the end of the sequence, then stop the repeat call to the OnNext() method and swith play/pause button visibility...
      
            ResetSequence();
          
          
        }
    }

    void ResizeCube(int index)
    {
        for (int i = 0; i < SequenceItems.Length; i++)
        {
            if (i == index)
                PhotonView.Find(SequenceItems[i].GetComponent<PhotonView>().ViewID).gameObject.transform.localScale = new Vector3(1f, 1.5f, 1f);
            else
                PhotonView.Find(SequenceItems[i].GetComponent<PhotonView>().ViewID).gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
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