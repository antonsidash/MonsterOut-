using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DisconnectController : MonoBehaviourPunCallbacks
{
    public void OnApplicationQuit()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected!");
        
        if (PhotonNetwork.PlayerListOthers.Length != 0)
            PhotonNetwork.SetMasterClient(PhotonNetwork.PlayerListOthers[0]);
    }
}
