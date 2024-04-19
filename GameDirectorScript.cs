using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class GameDirectorScript : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Проверяем на выигрыш, пока только по убийствам:
    public void CheckIsWin(PhotonView userPhotonView)
    {
        Debug.Log("[CheckIsWin]: Started!");
        // Определяем команду игрока:
        string Team = userPhotonView.Owner.CustomProperties["Team"].ToString();

        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        foreach (Player player in players)
        {
            if (player.CustomProperties["Team"].ToString() != Team)
                break;
            int playerViewID = GetPhotonViewIDByActorNumber(player.ActorNumber);
            if (playerViewID == -1)
                break;
            if (PhotonView.Find(playerViewID).gameObject.GetComponent<HealthController>().alive)
                return;
        }

        Debug.Log("[CheckIsWin]: TRUE!");
        //return true;
        foreach (Player player in players)
        {
            int playerViewID = GetPhotonViewIDByActorNumber(player.ActorNumber);
            if (playerViewID == -1)
                break;
            if (PhotonView.Find(playerViewID).gameObject.GetComponent<HealthController>().alive)
                PhotonView.Find(playerViewID).gameObject.GetComponent<PlayerUIDirectorScript>().Win();
        }
    }

    private int GetPhotonViewIDByActorNumber(int actorNumber)
    {
        foreach (PhotonView photonView in PhotonNetwork.PhotonViews)
            if (photonView.Owner.ActorNumber == actorNumber)
                return photonView.ViewID;
        return -1;
    }
}
