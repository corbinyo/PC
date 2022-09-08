using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RemoteCall : MonoBehaviour
{
    // Start is called before the first frame update
    private PhotonView myPV;

    [PunRPC]
    void Start()
    {
        myPV = GetComponent<PhotonView>();

    }
    [PunRPC]
    public void SoundYes(string note)
    {

     myPV.RPC("PlayBell", RpcTarget.All, note);
        Debug.Log("rpc sent");

    }
    [PunRPC]
    public void PlayBell(string note)
    {

        SerialCommunication.sendNote(note);
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
