using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MainScene : MonoBehaviour
{
    public GameObject container;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI promptText;
    //public InstructionText instructionText;
    public Image background;

    private Color normalBG;
    [SerializeField] private Color loseBG;
    [SerializeField] private Color winBG;

    private bool oldSpacePressed;
    private Action spacePressedAction;

    private String baseStatusText;

    private void Awake()
    {
        normalBG = background.color;
    }

    private void Start()
    {
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
    }

    private void OnEndMinigame()
    {
        container.SetActive(true);

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

        // flash a color if the game was won/lost

        if (status.nextMinigame != null)
        {
            // prepare for the next minigame
            DOVirtual.DelayedCall(1f, () =>
            {
                // return the background color to what it was before
                background.color = normalBG;

                // await input
                promptText.text = "Press SPACE to start next minigame";
                spacePressedAction = () => OnProceed(status, intermissionFinishedCallback);
            }, false);
        }
    }

    private void OnProceed(MinigameStatus status, Action intermissionFinishedCallback)
    {
        // start the sequence for the next minigame
        Debug.Log("space pressed!");
        //instructionText.ShowImpactText(status.nextMinigame.instruction);
        DOVirtual.DelayedCall(0.5f, () => intermissionFinishedCallback?.Invoke(), false);
    }


    /*
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        MainGameManager.Instance.GrowMainScene += GrowScene;
        MainGameManager.Instance.ShrinkMainScene += ShrinkScene;
    }

    private void GrowScene()
    {
        _animator.Play("main-scene-grow");
    }

    private void ShrinkScene()
    {
        _animator.Play("main-scene-shrink");
    }

    private void OnDestroy()
    {
        MainGameManager.Instance.GrowMainScene -= GrowScene;
        MainGameManager.Instance.ShrinkMainScene -= ShrinkScene;
    }
    */
}
