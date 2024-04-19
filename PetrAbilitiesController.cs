using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using StarterAssets;
using MonsterOutLibrary;

public class PetrAbilitiesController : MonoBehaviourPunCallbacks, AbilitiesController
{
    private GameDirectorScript _GameDirector;
    private PlayerUIDirectorScript _PlayerUIDirector;
    private NewThirdPersonController _ThirdPersonController;
    [SerializeField] private Camera _Camera;
    [SerializeField] private Transform AimPoint;
    // [SerializeField] GameObject CharacterModel;
    private GameObject Aim;
    private TextMeshProUGUI HPBar;
    private GameObject DeathMessage;
    public bool alive = true;
    [SerializeField] public int gunDamage = 20;
    [SerializeField] private float timeBetweenShots = 0.7f;
    private bool isGunAbilityActive = false;
    [SerializeField] private float increaseSpeedMultiplier = 1.4f;
    [SerializeField] private float increaseSpeedDuration = 4f;
    [SerializeField] private float fAbilityCooldown;
    private bool isFAbilityActive = false;
    [SerializeField] private float increaseDamageMultiplier = 1.2f;
    [SerializeField] private float increaseDamageDuration = 3f;
    [SerializeField] private float gun2AbilityCooldown;
    private bool isGun2AbilityActive = false;
    private Animator _Animator;
    //public AudioSource _AudioSource;
    [SerializeField] private AudioClip ShootAudioClip;
    [Range(0f, 1f)]
    [SerializeField] private float ShootVolume;
    [SerializeField] private LayerMask IgnoreShootLayers;

    [Header("Character Ability Icons")]
    [SerializeField] private Sprite Gun1AbilityIcon;
    [SerializeField] private Sprite Gun2AbilityIcon;
    [SerializeField] private Sprite FAbilityIcon;
    [SerializeField] private Sprite QAbilityIcon;
    [SerializeField] private Sprite UltAbilityIcon;

    // ! Нужно привязать все взаимодействия с интерфейсом к PlayerUIDirectorScript

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
            this.enabled = false;

        _GameDirector = GameObject.Find("GameDirector").GetComponent<GameDirectorScript>();
        _ThirdPersonController = GetComponent<NewThirdPersonController>();
        _PlayerUIDirector = GameObject.Find("GameDirector").GetComponent<PlayerUIDirectorScript>();
        Aim = GameObject.Find("Aim");
        //HPBar = GameObject.Find("HPBar").GetComponent<TextMeshProUGUI>();
        //HPBar.text = "+" + hp.ToString();
        //DeathMessage = _GameDirector.DeathMessage;
        _Animator = GetComponent<Animator>();
        //_AudioSource = GetComponent<AudioSource>();

        //_GameDirector.GetComponent<PlayerUIDirectorScript>();
        ConnectAbilityIcons();
    }

    // Update is called once per frame
    void Update()
    {
        FreeCamDirector();
        GunAbility();
        GunAbility2();
        AbilityF();
    }

    public void GunAbility()
    {
        if (Input.GetButtonDown("Fire1") && Aim.gameObject.active && !isGunAbilityActive)
        {
            RaycastHit hit;

            if (Physics.Raycast(AimPoint.position, _Camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f)).direction, out hit, 1000f, ~IgnoreShootLayers))
            {
                Debug.Log("Shooted: " + hit.transform.gameObject.name);
                if (hit.transform.gameObject.tag.Contains("Player"))
                {
                    int enemyViewID = hit.transform.gameObject.GetComponent<PhotonView>().ViewID;
                    photonView.RPC("ShootEnemy", RpcTarget.All, enemyViewID, gunDamage);
                }
            }

            AudioSource.PlayClipAtPoint(ShootAudioClip, transform.position, ShootVolume);
            isGunAbilityActive = true;
            StartCoroutine(BeginAbilityCooldown(AbilityType.Gun1, timeBetweenShots));
            photonView.RPC("ShootSound", RpcTarget.Others, photonView.ViewID);

            Debug.DrawRay(AimPoint.position, _Camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f)).direction, Color.red);
        }
    }

    public void FreeCamDirector()
    {
        if (Input.GetButtonDown("FreeCam") && !_ThirdPersonController._isRunning)
        {
            _Animator.SetBool("FreeCam", !_Animator.GetBool("FreeCam"));
            Aim.gameObject.SetActive(!_Animator.GetBool("FreeCam"));
        }
    }

    [PunRPC]
    private void ShootEnemy(int viewID, int damage)
    {
        PhotonView shootedPlayer = PhotonView.Find(viewID);
        shootedPlayer.gameObject.GetComponent<HealthController>().HP -= damage;
    }

    [PunRPC]
    private void ShootSound(int viewID)
    {
        // Нужно исправить, ибо если стреляющий игрок не будет Петром, вылезет ошибка:
        AudioSource.PlayClipAtPoint(ShootAudioClip, PhotonView.Find(viewID).gameObject.transform.position, ShootVolume);
    }

    public void GunAbility2()
    {
        if (Input.GetButtonDown("Fire2") && !isGun2AbilityActive)
        {
            StartCoroutine(IncreaseDamage());
            isGun2AbilityActive = true;
            StartCoroutine(BeginAbilityCooldown(AbilityType.Gun2, gun2AbilityCooldown));
        }
    }

    // Так же нужна анимация самого поедания "Сникерса"
    public void AbilityF()
    {
        if (Input.GetButtonDown("AbilityF"))
        {
            Debug.Log("F ability available:" + isFAbilityActive);
            Debug.Log("Now base speed:" + _ThirdPersonController.MoveSpeed);
        }

        if (Input.GetButtonDown("AbilityF") && !isFAbilityActive)
        {
            StartCoroutine(IncreaseSpeed());
            isFAbilityActive = true;
            StartCoroutine(BeginAbilityCooldown(AbilityType.F, fAbilityCooldown));
        }
    }

    private IEnumerator IncreaseSpeed()
    {
        float basicSpeed = _ThirdPersonController.MoveSpeed;
        float basicSpeedSprint = _ThirdPersonController.SprintSpeed;

        _ThirdPersonController.MoveSpeed *= increaseSpeedMultiplier;
        _ThirdPersonController.SprintSpeed *= increaseSpeedMultiplier;
        float basicAnimatorSpeed = _Animator.speed;
        _Animator.speed *= increaseSpeedMultiplier;

        yield return new WaitForSeconds(increaseSpeedDuration);

        _ThirdPersonController.MoveSpeed = basicSpeed;
        _ThirdPersonController.SprintSpeed = basicSpeedSprint;
        _Animator.speed = basicAnimatorSpeed;
    }

    private IEnumerator IncreaseDamage()
    {
        int basicGunDamage = gunDamage;
        gunDamage = (int)(gunDamage * increaseDamageMultiplier);

        yield return new WaitForSeconds(increaseDamageDuration);

        gunDamage = basicGunDamage;
    }

    private IEnumerator BeginAbilityCooldown(AbilityType aType, float cooldown)
    {
        // Запуск анимации:
        _PlayerUIDirector.BeginFAbilityCooldownAnim(aType, cooldown);

        yield return new WaitForSeconds(cooldown);

        switch (aType)
        {
            case AbilityType.Gun1: { isGunAbilityActive = false; break; }
            case AbilityType.Gun2: { isGun2AbilityActive = false;  break; }
            case AbilityType.Q: { break; }
            case AbilityType.F: { isFAbilityActive = false; break;  }
            case AbilityType.Ult: { break; }
        }
    }

    public void AbilityQ()
    {
    }

    public void Ultimate()
    {
    }

    private void ConnectAbilityIcons()
    {
        _PlayerUIDirector.Gun1AbilityIcon.sprite = Gun1AbilityIcon;
        _PlayerUIDirector.Gun2AbilityIcon.sprite = Gun2AbilityIcon;
        _PlayerUIDirector.FAbilityIcon.sprite = FAbilityIcon;
        _PlayerUIDirector.QAbilityIcon.sprite = QAbilityIcon;
        _PlayerUIDirector.UltAbilityIcon.sprite = UltAbilityIcon;
    }
}
