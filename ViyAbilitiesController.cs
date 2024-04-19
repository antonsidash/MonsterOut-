using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using StarterAssets;
using MonsterOutLibrary;

public class ViyAbilitiesController : MonoBehaviourPunCallbacks, AbilitiesController
{
    private GameDirectorScript _GameDirector;
    private NewThirdPersonController _ThirdPersonController;
    private PlayerUIDirectorScript _PlayerUIDirector;
    [SerializeField] private Camera _Camera;
    [SerializeField] private Transform AimPoint;
    // [SerializeField] GameObject CharacterModel;
    private GameObject Aim;
    private TextMeshProUGUI HPBar;
    private GameObject DeathMessage;
    public bool alive = true;
    [SerializeField] public int gunDamage = 20;
    [SerializeField] private float timeBetweenShots = 0.2f;
    private bool isGunAbilityActive = false;
    private Animator _Animator;
    //public AudioSource _AudioSource;
    [SerializeField] private AudioClip ShootAudioClip;
    [Range(0f, 1f)]
    [SerializeField] private float ShootVolume;
    [SerializeField] private LayerMask IgnoreShootLayers;

    [SerializeField] private float increaseSpeedMultiplier = 1.4f;
    [SerializeField] private float increaseSpeedDuration = 4f;
    [SerializeField] private float fAbilityCooldown;
    private bool isFAbilityActive = false;

    [Header("Shooting")]
    [SerializeField] private float MaxAttackDistance = 0.5f;
    [SerializeField] private float AttackRadius = 0.5f;

    [Header("Character Ability Icons")]
    [SerializeField] private Sprite Gun1AbilityIcon;
    [SerializeField] private Sprite Gun2AbilityIcon;
    [SerializeField] private Sprite FAbilityIcon;
    [SerializeField] private Sprite QAbilityIcon;
    [SerializeField] private Sprite UltAbilityIcon;

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
            this.enabled = false;

        _GameDirector = GameObject.Find("GameDirector").GetComponent<GameDirectorScript>();
        _ThirdPersonController = GetComponent<NewThirdPersonController>();
        _PlayerUIDirector = GameObject.Find("GameDirector").GetComponent<PlayerUIDirectorScript>();
        //HPBar = GameObject.Find("HPBar").GetComponent<TextMeshProUGUI>();
        //HPBar.text = "+" + hp.ToString();
        //DeathMessage = _GameDirector.DeathMessage;
        _Animator = GetComponent<Animator>();
        //_AudioSource = GetComponent<AudioSource>();

        ConnectAbilityIcons();
    }

    // Update is called once per frame
    void Update()
    {
        GunAbility();
        AbilityF();
    }

    public void GunAbility()
    {
        // Пока что SphereCast, далее будет через Trigger перед персонажем
        if (Input.GetButtonDown("Fire1") && !isGunAbilityActive)
        {
            RaycastHit hit;

            Vector3 origin = AimPoint.position;
            Vector3 direction = _Camera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f)).direction;
            LayerMask layerMask = ~IgnoreShootLayers;
            
            if (Physics.SphereCast(origin, AttackRadius, direction, out hit, MaxAttackDistance, layerMask))
            {
                Debug.Log("Shooted: " + hit.transform.gameObject.name);
                if (hit.transform.gameObject.tag.Contains("Player"))
                {
                    int enemyViewID = hit.transform.gameObject.GetComponent<PhotonView>().ViewID;
                    photonView.RPC("ShootEnemy", RpcTarget.All, enemyViewID, gunDamage);
                }
            }

            isGunAbilityActive = true;
            StartCoroutine(BeginAbilityCooldown(AbilityType.Gun1, timeBetweenShots));
            _Animator.SetTrigger("Attack");
        }
    }

    [PunRPC]
    private void ShootEnemy(int viewID, int damage)
    {
        PhotonView shootedPlayer = PhotonView.Find(viewID);
        shootedPlayer.gameObject.GetComponent<HealthController>().HP -= damage;
    }

    public void GunAbility2()
    {
    }

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

    public void AbilityQ()
    {
    }

    public void Ultimate()
    {
    }

    private IEnumerator BeginAbilityCooldown(AbilityType aType, float cooldown)
    {
        // Запуск анимации:
        _PlayerUIDirector.BeginFAbilityCooldownAnim(aType, cooldown);

        yield return new WaitForSeconds(cooldown);

        switch (aType)
        {
            case AbilityType.Gun1: { isGunAbilityActive = false; break; }
            case AbilityType.Gun2: { break; }
            case AbilityType.Q: { break; }
            case AbilityType.F: { isFAbilityActive = false; break; }
            case AbilityType.Ult: { break; }
        }
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
