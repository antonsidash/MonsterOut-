using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using MonsterOutLibrary;

public class PlayerLobbyPanel : MonoBehaviourPunCallbacks
{
    // enum Team { Survivors, Monsters};

    [SerializeField] TextMeshProUGUI NickName;
    [SerializeField] TextMeshProUGUI CharacterNameText;
    [SerializeField] GameObject SelectCharacterButton;
    // CharacterAvatar;

    private RectTransform[] SurvivorsPanelsTransforms;
    private RectTransform[] MonstersPanelsTransforms;

    private GameObject LobbyDirector;

    private GameObject SurvivorsPanel;
    private GameObject MonstersPanel;
    private GameObject EnterSurivorsButton;
    private GameObject EnterMonstersButton;
    private GameObject ReadyButton;
    private GameObject StartMatchButton;


    private void Awake()
    {
        LobbyDirector = GameObject.Find("LobbyDirector");
        SurvivorsPanelsTransforms = LobbyDirector.GetComponent<LobbyDirector>().SurvivorsPanelsTransforms;
        MonstersPanelsTransforms = LobbyDirector.GetComponent<LobbyDirector>().MonstersPanelsTransforms;

        SurvivorsPanel = GameObject.Find("Survivors Panel");
        MonstersPanel = GameObject.Find("Monsters Panel");
        EnterSurivorsButton = GameObject.Find("Enter Survivors");
        EnterMonstersButton = GameObject.Find("Enter Monsters");
        ReadyButton = LobbyDirector.GetComponent<LobbyDirector>().ReadyButton;
        StartMatchButton = LobbyDirector.GetComponent<LobbyDirector>().StartMatchButton;


        if (photonView.IsMine)
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("NickName");
            NickName.text = PhotonNetwork.NickName;

            // Привязываем к кнопки "Войти за Выживших" скрипт:
            EnterSurivorsButton.GetComponent<Button>().onClick.AddListener(EnterSurvivors);
            EnterMonstersButton.GetComponent<Button>().onClick.AddListener(EnterMonsters);
            ReadyButton.GetComponent<Button>().onClick.AddListener(Ready);
            StartMatchButton.GetComponent<Button>().onClick.AddListener(StartMatchClick);

            if (PhotonNetwork.LocalPlayer.IsMasterClient)
                StartMatchButton.SetActive(true);
        }
    }

    public void EnterSurvivors()
    {
        // Привязываем панель игрока к панели Выживших, и подстраиваем её координаты:
        Debug.Log("Enter Survivors started!");

        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team") && PhotonNetwork.LocalPlayer.CustomProperties["Team"].ToString() == Team.Survivors.ToString())
        {
            Debug.Log("Ваш номер команды совпадает с тем, который вы хотите выбрать!");
            return;
        }

        photonView.RPC("SelectTeam", RpcTarget.AllBuffered, photonView.ViewID, Team.Survivors);
        SelectCharacterButton.SetActive(true);
        ReadyButton.SetActive(true);
    }

    public void EnterMonsters()
    {
        // Привязываем панель игрока к панели Выживших, и подстраиваем её координаты:
        Debug.Log("Enter Monsters started!");

        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team") && PhotonNetwork.LocalPlayer.CustomProperties["Team"].ToString() == Team.Monsters.ToString())
        {
            Debug.Log("Ваш номер команды совпадает с тем, который вы хотите выбрать!");
            return;
        }

        photonView.RPC("SelectTeam", RpcTarget.AllBuffered, photonView.ViewID, Team.Monsters);
        SelectCharacterButton.SetActive(true);
        ReadyButton.SetActive(true);
    }

    [PunRPC]
    private void SelectTeam(int viewID, Team team)
    {
        if (team == Team.Survivors)
        {
            foreach (RectTransform i in SurvivorsPanelsTransforms)
                if (i.childCount == 0)
                {
                    PhotonNetwork.GetPhotonView(viewID).gameObject.GetComponent<PlayerLobbyPanel>().NickName.text = PhotonNetwork.GetPhotonView(viewID).Owner.NickName;
                    PhotonNetwork.GetPhotonView(viewID).transform.parent = SurvivorsPanel.transform;
                    PhotonNetwork.GetPhotonView(viewID).gameObject.GetComponent<RectTransform>().position = i.position;
                    PhotonNetwork.GetPhotonView(viewID).gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, i.rect.size.x);
                    PhotonNetwork.GetPhotonView(viewID).gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, i.rect.size.y);
                    PhotonNetwork.GetPhotonView(viewID).gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                    PhotonNetwork.GetPhotonView(viewID).transform.parent = i.transform;
                    PhotonNetwork.GetPhotonView(viewID).gameObject.SetActive(true);
                    PhotonNetwork.GetPhotonView(viewID).Owner.CustomProperties["Team"] = team;
                    // [Временно] Автоматически выбираем Петра как персонажа:
                    PhotonNetwork.GetPhotonView(viewID).Owner.CustomProperties["Character"] = "Petr";
                    return;
                }
        }
        else
        {
            foreach (RectTransform i in MonstersPanelsTransforms)
                if (i.childCount == 0)
                {
                    PhotonNetwork.GetPhotonView(viewID).gameObject.GetComponent<PlayerLobbyPanel>().NickName.text = PhotonNetwork.GetPhotonView(viewID).Owner.NickName;
                    PhotonNetwork.GetPhotonView(viewID).transform.parent = SurvivorsPanel.transform;
                    PhotonNetwork.GetPhotonView(viewID).gameObject.GetComponent<RectTransform>().position = i.position;
                    PhotonNetwork.GetPhotonView(viewID).gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, i.rect.size.x);
                    PhotonNetwork.GetPhotonView(viewID).gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, i.rect.size.y);
                    PhotonNetwork.GetPhotonView(viewID).gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                    PhotonNetwork.GetPhotonView(viewID).transform.parent = i.transform;
                    PhotonNetwork.GetPhotonView(viewID).gameObject.SetActive(true);
                    PhotonNetwork.GetPhotonView(viewID).Owner.CustomProperties["Team"] = team;
                    PhotonNetwork.GetPhotonView(viewID).Owner.CustomProperties["Character"] = "Viy";
                    return;
                }
        }
    }

    public void Ready()
    {
        return;
    }
    public void StartMatchClick()
    {
        photonView.RPC("StartMatch", RpcTarget.All);
    }

    [PunRPC]
    public void StartMatch()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
}
