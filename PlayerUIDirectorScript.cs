using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using MonsterOutLibrary;

public class PlayerUIDirectorScript : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI HPBar;
    [SerializeField] private TMPro.TextMeshProUGUI DeathMessage;
    [SerializeField] private TMPro.TextMeshProUGUI WinMessage;

    [SerializeField] public UICanvasControllerInput _UICanvasControllerInput;

    [Header("Ability Panel")]
    [SerializeField] public Image Gun1AbilityIcon;
    [SerializeField] public Image Gun2AbilityIcon;
    [SerializeField] public Image FAbilityIcon;
    [SerializeField] public Image QAbilityIcon;
    [SerializeField] public Image UltAbilityIcon;
    //[SerializeField] public MobileDisableAutoSwitchControls _MobileDisableAutoSwitchControls;

    private Image ScreenFlash;

    // Start is called before the first frame update
    void Start()
    {
        ScreenFlash = GameObject.Find("ScreenFlash").GetComponent<Image>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeHPBar(int newHP)
    {
        HPBar.text = "+" + newHP.ToString();
    }

    public void OnDie()
    {
        HPBar.gameObject.SetActive(false);
        DeathMessage.gameObject.SetActive(true);
    }

    public IEnumerator FlashScreenAnimation()
    {
        ScreenFlash.color = new Color(ScreenFlash.color.r, ScreenFlash.color.g, ScreenFlash.color.b, 0.3f); // Устанавливаем цвет покраснения

        yield return new WaitForSeconds(0.1f); // Ждем некоторое время

        ScreenFlash.color = new Color(ScreenFlash.color.r, ScreenFlash.color.g, ScreenFlash.color.b, 0f); ; // Устанавливаем прозрачный цвет
    }

    public void Win()
    {
        WinMessage.gameObject.SetActive(true);
    }

    public void BeginFAbilityCooldownAnim(AbilityType aType, float duration)
    {
        // Предполагается, что длина каждой анимации 60 кадров — для правильного расчета времени проигрывания
        switch (aType)
        {
            case AbilityType.Gun1: 
                {
                    Gun1AbilityIcon.gameObject.GetComponent<Animator>().speed = 1f / duration;
                    Gun1AbilityIcon.gameObject.GetComponent<Animator>().SetTrigger("AbilityCooldownStarted");
                    break; ; 
                }
            case AbilityType.Gun2: 
                {
                    Gun2AbilityIcon.gameObject.GetComponent<Animator>().speed = 1f / duration;
                    Gun2AbilityIcon.gameObject.GetComponent<Animator>().SetTrigger("AbilityCooldownStarted");
                    break; ; 
                }
            case AbilityType.Q: 
                {
                    QAbilityIcon.gameObject.GetComponent<Animator>().speed = 1f / duration;
                    QAbilityIcon.gameObject.GetComponent<Animator>().SetTrigger("AbilityCooldownStarted");
                    break; ; 
                }
            case AbilityType.F: 
                {
                    FAbilityIcon.gameObject.GetComponent<Animator>().speed = 1f / duration;
                    FAbilityIcon.gameObject.GetComponent<Animator>().SetTrigger("AbilityCooldownStarted");
                    break;
                }
            case AbilityType.Ult: 
                {
                    UltAbilityIcon.gameObject.GetComponent<Animator>().speed = 1f / duration;
                    UltAbilityIcon.gameObject.GetComponent<Animator>().SetTrigger("AbilityCooldownStarted");
                    break; ; 
                }
        }
    }
}