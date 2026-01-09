using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Spine.Unity;

public class MainScene : MonoBehaviour
{
    //container not made yet
    public GameObject container;
    public GameObject deerScreen;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI promptText;
    public InstructionText instructionText;
    public Image background;

    private Color normalBG;
    [SerializeField] private Color loseBG;
    [SerializeField] private Color winBG;

    private bool oldSpacePressed;
    private Action spacePressedAction;

    private String baseStatusText;

    //animations
    private SkeletonAnimation deerAnimator;
    private Animator explosionAnimator;
    private ParticleSystem dustAnimaiton;
    private GameObject encounterObject;
    private Animator encounterObjAnimator;
    private Animator _animator;

    public Animator healthUI;
    public Animator progressUI;
    public Animator statsUI;
    public Animator livesUI;
    private void Awake()
    {
        normalBG = background.color;
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        deerAnimator = deerScreen.transform.GetChild(1).GetComponent<SkeletonAnimation>();
        encounterObject = deerScreen.transform.GetChild(3).gameObject;
        encounterObjAnimator = encounterObject.GetComponent<Animator>();

        explosionAnimator = deerScreen.transform.GetChild(4).GetComponent<Animator>();
        dustAnimaiton = deerScreen.transform.GetChild(5).GetComponent<ParticleSystem>();
        StartCoroutine("Startup");
    }

    private IEnumerator Startup()
    {
        while (!Managers.__instance) { yield return null; }
        while (!Managers.__instance.minigamesManager) { yield return null; }
        Managers.__instance.minigamesManager.OnStartMinigame += OnStartMinigame;
        Managers.__instance.minigamesManager.OnEndMinigame += OnEndMinigame;
        Managers.__instance.minigamesManager.OnBeginIntermission += OnBeginIntermission;
        Managers.__instance.minigamesManager.StartMinigames();
    }

    private void OnDestroy()
    {
        Managers.__instance.minigamesManager.OnStartMinigame -= OnStartMinigame;
        Managers.__instance.minigamesManager.OnEndMinigame -= OnEndMinigame;
        Managers.__instance.minigamesManager.OnBeginIntermission -= OnBeginIntermission;
    }


    private void Update()
    {
        // call the space pressed action whenever space is pressed
        bool spacePressed = Input.GetAxis("Space") > 0;
        if (spacePressed && !oldSpacePressed)
        {
            spacePressedAction?.Invoke();
            spacePressedAction = null;
        }

        float axis = Input.GetAxis("Horizontal");

        SetStatusText();

        oldSpacePressed = spacePressed;
    }


    private void OnStartMinigame(MinigameDefinition _)
    {

        container.SetActive(false);
        deerScreen.SetActive(false);
    }

    private void OnEndMinigame()
    {
        container.SetActive(true);
        deerScreen.SetActive(true);

        // reset the prompt text
        promptText.text = "";
    }

    public void SetDifficulty(Slider s)
    {
        Managers.__instance.minigamesManager.minigameDifficulty = s.value;
        SetStatusText();
    }

    private void SetStatusText()
    {
        if (!Managers.__instance) return;
        String statusTextString =
            baseStatusText + $"\nCurrent Difficulty: {Managers.__instance.minigamesManager.minigameDifficulty.ToString()} (use slider to adjust)";

        //statusText.text = statusTextString;
    }

    private void OnBeginIntermission(MinigameStatus status, Action intermissionFinishedCallback)
    {
        // write all of the status to the screen

        baseStatusText =
            $"Result of previous minigame: {(status.previousMinigameResult == WinLose.WIN ? "Won" : status.previousMinigameResult == WinLose.LOSE ? "Lost" : "N/A")}\n" +
            $"Lives: {Managers.__instance.minigamesManager.lives}\n" +
            $"Overall game status: {(status.gameResult == WinLose.WIN ? "Won" : status.gameResult == WinLose.LOSE ? "Lost" : "Playing")}";

        SetStatusText();

        updateDeerAnimation(status);

        if (status.nextMinigame != null)
        {
            // prepare for the next minigame
            DOVirtual.DelayedCall(1f, () =>
            {
                // return the background color to what it was before
                background.color = normalBG;
                if (!Managers.__instance.minigamesManager.isBoss)
                {
                    // await input
                    promptText.text = "Press SPACE to start next minigame";
                    spacePressedAction = () => OnProceed(status, intermissionFinishedCallback);
                }
                else
                {
                    OnProceed(status, intermissionFinishedCallback);
                }

            }, false);
        }
    }

    public AudioClip zoomSound;
    private void OnProceed(MinigameStatus status, Action intermissionFinishedCallback)
    {
        // start the sequence for the next minigame
        Debug.Log("space pressed!");
        _animator.SetBool("endgame", false);

        Managers.__instance.minigamesManager.triggerUIExit();

        //StartCoroutine(startMiniGameAnimation());
        var track = deerAnimator.AnimationState.SetAnimation(0, "THINKING", false);
        float triggerTime = Mathf.Max(0, track.Animation.Duration - 0.25f);



        PlayerPrefs.SetFloat("minigameLength", (float)status.nextMinigame.gameTime);
        instructionText.ShowImpactText(status.nextMinigame.instruction);
        DOVirtual.DelayedCall(1f, () =>
        {
            _animator.SetBool("intogame", true);
            Managers.__instance.audioManager.PlaySFX(zoomSound);
        }, false);
        DOVirtual.DelayedCall(triggerTime, () => intermissionFinishedCallback?.Invoke(), false);

        if (Managers.__instance.minigamesManager.isBoss)
        {
            //animation
        }
    }

    private void updateDeerAnimation(MinigameStatus status)
    {
        //assembly
        if (status.previousMinigame != null)
        {
            _animator.SetBool("endgame", true);

            deerAnimator.AnimationState.SetAnimation(0, "ASSEMBLING", false);
            dustAnimaiton.Play();

            //result
            switch (status.previousMinigameResult)
            {
                case WinLose.WIN:
                    deerAnimator.AnimationState.AddAnimation(0, "SUCCESS", false, 0f);
                    StartCoroutine(timerChangeObject(true, 1f));
                    break;
                case WinLose.LOSE:
                    var track = deerAnimator.AnimationState.AddAnimation(0, "EXPLOSION", false, 0f);
                    float spineDuration = track.Animation.Duration;
                    float leadTime = 2.8f;
                    float delayTime = Mathf.Max(0, spineDuration - leadTime);
                    DG.Tweening.DOVirtual.DelayedCall(delayTime, () =>
                    {
                        explosionAnimator.SetBool("explosion", true);
                    });
                    break;
                default:
                    deerAnimator.AnimationState.AddAnimation(0, "IDLE", true, 0f);
                    break;
            }
            StartCoroutine(endSequence());

        }
    }

    IEnumerator endSequence()
    {
        yield return new WaitForSeconds(2f);

        encounterObjAnimator.SetTrigger("end");
        explosionAnimator.SetBool("explosion", false);

        StartCoroutine(timerChangeObject(false, 1.3f));
    }

    IEnumerator startMiniGameAnimation()
    {
        deerAnimator.AnimationState.SetAnimation(0, "THINKING", false);
        yield return new WaitForSeconds(5f);

    }

    IEnumerator timerChangeObject(bool change, float timer)
    {
        yield return new WaitForSeconds(timer);
        encounterObject.GetComponent<EncounterObject>().changeType(change);
    }


}
