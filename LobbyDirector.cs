using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class LobbyDirector : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject PlayerLobbyPanel;
    [SerializeField] private GameObject SurvivorsPanel;
    [SerializeField] private GameObject MonsterPanel;
    [SerializeField] private TextMeshProUGUI PlayerList;
    [SerializeField] public GameObject ReadyButton;
    [SerializeField] public GameObject StartMatchButton;

    [SerializeField] public RectTransform[] SurvivorsPanelsTransforms;
    [SerializeField] public RectTransform[] MonstersPanelsTransforms;

    public void Start()
    {
        PhotonNetwork.Instantiate("PlayerLobbyPanel", Vector3.zero, Quaternion.identity);

        Debug.Log("PlayerList: " + PhotonNetwork.PlayerList.ToStringFull());
    }

    public void FixedUpdate()
    {
        PlayerList.text = "В комнате: " + PhotonNetwork.PlayerList.ToStringFull();
    }

    public void EnterSurvivorsButton()
    {
        //photonView.RPC("ChoiceTeamSurvivors", RpcTarget.All, PhotonNetwork.LocalPlayer.UserId, photonView.ViewID);

        //PhotonNetwork.LocalPlayer.ActorNumber;

        //PhotonView myPlayerPhotonView = PhotonView.Find(PhotonNetwork.CurrentRoom.GetPlayer(1).ActorNumber);
        //Debug.Log("My Photon ViewID:" + myPlayerPhotonView.ViewID);;
    }

    /*[PunRPC]
    private void ChoiceTeamSurvivors(string userID, int viewID)
    {
        Debug.Log("Received UserID: " + userID);
        Debug.Log("Received ViewID: " + viewID.ToString());

        
    }*/

    public void EnterMonstersButton()
    {

    }
}
