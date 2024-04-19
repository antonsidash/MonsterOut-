using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomJoin : MonoBehaviourPunCallbacks
{
    [SerializeField] TMPro.TextMeshProUGUI NickNameText;
    [SerializeField] TMP_InputField NickNameField;

    public void CreateRoom()
    {
        Debug.Log("Server: " + PhotonNetwork.Server);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 5;
        PhotonNetwork.CreateRoom("room1", roomOptions);
        Debug.Log("Room created!");

        Debug.Log("Server: " + PhotonNetwork.Server);
    }

    public void JoinRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinRoom("room1");
            Debug.Log("Room joined!");
            Debug.Log("Server: " + PhotonNetwork.Server);
        }
        else
        {
            Debug.Log("Not Connected or not Ready!");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom start!");
        PhotonNetwork.LoadLevel("MatchLobby");
        Debug.Log("OnJoinedRoom end!");
    }

    private void Start()
    {
        Debug.Log("Start PlayerPrefs NickName: " + PlayerPrefs.GetString("NickName"));

        if (!PlayerPrefs.HasKey("NickName"))
            PlayerPrefs.SetString("NickName", "UnknownUser");
        else
        {
            Debug.Log("Start PlayerPrefs NickName: " + PlayerPrefs.GetString("NickName"));
            NickNameField.text = PlayerPrefs.GetString("NickName");
        }
    }


    public void OnNickNameEndEdit()
    {
        if (NickNameField.text != "")
            PlayerPrefs.SetString("NickName", NickNameField.text);

        Debug.Log("End Edit");
    }
}