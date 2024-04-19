using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using StarterAssets;
using UnityEngine.InputSystem;

public class HealthController : MonoBehaviourPunCallbacks
{
    private GameObject GameDirector;
    private GameDirectorScript _GameDirectorScript;
    private PlayerUIDirectorScript _PlayerUIDirectorScript;
    [SerializeField] private GameObject CharacterModel;
    public bool alive = true;

    public int hp = 100;
    public int HP
    {
        get { return hp; }
        set
        {
            hp = value;
            if (photonView.IsMine)
            {
                OnHPChanged();
                StartCoroutine(_PlayerUIDirectorScript.FlashScreenAnimation());
            }
        }
    }

    private void OnHPChanged()
    {
        _PlayerUIDirectorScript.ChangeHPBar(hp);

        if (hp <= 0)
        {
            //_GameDirectorScript.CheckIsWin(photonView);
            Debug.Log("[HealthControler]: U DIE!");
            photonView.RPC("PlayerDeath", RpcTarget.Others, photonView.ViewID);
            CharacterModel.SetActive(false);
            alive = false;
            //Aim.gameObject.SetActive(false);
            _PlayerUIDirectorScript.OnDie();
        }
    }

    [PunRPC]
    private void PlayerDeath(int viewID)
    {
        PhotonView.Find(viewID).gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        GameDirector = GameObject.Find("GameDirector");
        _GameDirectorScript = GameDirector.GetComponent<GameDirectorScript>();
        _PlayerUIDirectorScript = GameDirector.GetComponent<PlayerUIDirectorScript>();

        _PlayerUIDirectorScript._UICanvasControllerInput.starterAssetsInputs = GetComponent<StarterAssetsInputs>();

        //_PlayerUIDirectorScript.gameObject.GetComponent<MobileDisableAutoSwitchControls>().playerInput = GetComponent<PlayerInput>();

        _PlayerUIDirectorScript.ChangeHPBar(hp);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
