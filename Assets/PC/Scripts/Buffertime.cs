using Photon.Pun;
using UnityEngine;

public class Buffertime : MonoBehaviourPun, IPunObservable
{


    Vector3 latestPos;
    Quaternion latestRot;
    //Lag compensation
    float currentTime = 0;
    double currentPacketTime = 0;
    double lastPacketTime = 0;
    Vector3 positionAtLastPacket = Vector3.zero;
    Quaternion rotationAtLastPacket = Quaternion.identity;
    public bool teleportIfFar;
    public float teleportIfFarDistance;
    [Header("Lerping[Experimental]")]
    public float smoothpos = 0.5f;
    public float smoothRot = 0.5f;
    void Awake()
    {
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 10;

    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();

            //Lag compensation
            currentTime = 0.0f;
            lastPacketTime = currentPacketTime;
            currentPacketTime = info.SentServerTime;
            positionAtLastPacket = transform.position;
            rotationAtLastPacket = transform.rotation;

        }
    }
    void FixedUpdate()
    {
        if (photonView.IsMine) return;
        double timeToReachGoal = currentPacketTime - lastPacketTime;
        currentTime += Time.deltaTime;

        //Update remote player
        transform.position = Vector3.Lerp(positionAtLastPacket, latestPos, (float)(currentTime / timeToReachGoal));
        transform.rotation = Quaternion.Lerp(rotationAtLastPacket, latestRot, (float)(currentTime / timeToReachGoal));

        if (Vector3.Distance(transform.position, latestPos) > teleportIfFarDistance)
        {
            transform.position = latestPos;
        }
    }
}