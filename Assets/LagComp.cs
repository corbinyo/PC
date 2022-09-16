using UnityEngine;
using Photon.Pun;
using TMPro;
public class LagComp : MonoBehaviourPun, IPunObservable
{
    //Values that will be synced over network
    Vector3 latestPos;
    Quaternion latestRot;
    //Lag compensation
    float currentTime = 0;
    double currentPacketTime = 0;
    double lastPacketTime = 0;
    Vector3 positionAtLastPacket = Vector3.zero;
    Quaternion rotationAtLastPacket = Quaternion.identity;
    private int _currentIndex;
    public GameObject[] SequenceItems;
    private TextMeshPro textMesh = null;
    Vector3 GlobalScore;
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(CurrentIndex);
            stream.SendNext(transform.rotation);
        }
        else
        {
            //Network player, receive data
            latestPos.x = (int)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();

            //Lag compensation
            currentTime = 0.0f;
            lastPacketTime = currentPacketTime;
            currentPacketTime = info.SentServerTime;
            positionAtLastPacket.x = CurrentIndex;
            rotationAtLastPacket = transform.rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            //Lag compensation
            double timeToReachGoal = currentPacketTime - lastPacketTime;
            currentTime += Time.deltaTime;

            //Update remote player
            GlobalScore = Vector3.Lerp(positionAtLastPacket, latestPos, (float)(currentTime / timeToReachGoal));
            transform.rotation = Quaternion.Lerp(rotationAtLastPacket, latestRot, (float)(currentTime / timeToReachGoal));
        }
    }
}