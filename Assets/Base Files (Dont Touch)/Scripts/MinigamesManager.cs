using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static IMinigamesManager;

public class MinigamesManager : MonoBehaviour, IMinigamesManager
{
    public const int STARTING_LIVES = 3;

    [SerializeField] private List<MinigameDefinition> allMinigames;
    [SerializeField] private List<MinigameDefinition> skillMinigames;
    [SerializeField] private List<MinigameDefinition> timingMinigames;

    public Action<MinigameStatus, Action> OnBeginIntermission;
    public Action<MinigameDefinition> OnStartMinigame;
    public Action OnEndMinigame;

    public float encounterHealth;
    public float maxHealth;
    public float currProgressBar;
    public float tgtProgressBar;
    public int lives;

    //encounter stats
    private int eliteStats;
    private float critChance;
    private float damage;
    public int encounterNum;

    //Stats UI
    public TextMeshProUGUI showDamage;
    public TextMeshProUGUI showCritChance;
    public Slider healthSlider;
    public Slider progSlider;

    int minigameIndex;
    List<MinigameDefinition> minigamePool;

    private UpgradeManager upgradeManager
    {
        get
        {
            return Managers.__instance.upgradeManager;
        }
    }

    public float minigameDifficulty;

    private MinigameStatus minigameStatus;
    private EncounterStatus encounterStatus;

    private bool isMinigamePlaying;
    private bool isCurrentMinigameWon;

    private Coroutine minigameEndCoroutine;

    //TODO Tomorrow: small 1.Finish Life UI 2.Add encounter count UI 3.Check for inconsistencies 4.Mark placeholders & animations 5.Mark mainScene for adaptations
    //important: finish loadnextencounter and integrade choicer.
    public void Initialize()
    {
        isMinigamePlaying = false;
        isCurrentMinigameWon = false;
        minigameIndex = 0;
        lives = 3;
    }

    public void StartMinigames()
    {
        upgradeManager.EncounterStart(); //call stats, gets maxHealth, crit, damage
        //choicer called 1st time to get, gets difficculty, type
        maxHealth = upgradeManager.Health;
        critChance = upgradeManager.CritChance;
        damage = upgradeManager.Damage;
        encounterHealth = maxHealth;
        tgtProgressBar = minigameDifficulty * 100; //scaling hand over to donn
        currProgressBar = 0;
        //Update stats UI accordingly
        UpdatePlayerStatsUI();

        //Update minigame pool/index appropriately
        if (encounterStatus.encounterType == EncounterType.SKILL)
        {
            minigamePool = skillMinigames;
        }
        else if (encounterStatus.encounterType == EncounterType.TIME)
        {
            minigamePool = timingMinigames;
        }
        else
        {
            minigamePool = allMinigames;
        }

        minigameStatus.nextMinigame = minigamePool[minigameIndex];
        RunIntermission(minigameStatus);
    }

    public void DeclareCurrentMinigameWon()
    {
        if (!isMinigamePlaying)
            return;
        isCurrentMinigameWon = true;
    }

    public void DeclareCurrentMinigameLost()
    {
        if (!isMinigamePlaying)
            return;
        isCurrentMinigameWon = false;
    }

    public float GetCurrentMinigameDifficulty()
    {
        return Mathf.Clamp01(minigameDifficulty);
    }

    public void EndCurrentMinigame(float delay = 0)
    {
        if (!isMinigamePlaying)
        {
            Debug.LogWarning("EndCurrentMinigame is called when a minigame is not being played. This might happen if you try to call EndCurrentMinigame right after the minigame ran out of time. This call will be ignored.");
            return;
        }

        if (minigameEndCoroutine != null)
        {
            Debug.LogError("Attempt to call EndCurrentMinigame more than once!");
            return;
        }

        minigameEndCoroutine = StartCoroutine(DoEndMinigame(delay));
    }

    // used by the timer to end a minigame regardless of whether the minigame has been ended by itself
    public void ForceEndCurrentMinigame()
    {
        if (!isMinigamePlaying)
        {
            Debug.LogError("Attempt to call ForceEndCurrentMinigame when a minigame is not being played!");
            return;
        }

        if (minigameEndCoroutine != null)
        {
            StopCoroutine(minigameEndCoroutine);
        }
        minigameEndCoroutine = StartCoroutine(DoEndMinigame(0));
    }

    private IEnumerator DoEndMinigame(float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        isMinigamePlaying = false;
        OnEndMinigame?.Invoke();

        Managers.__instance.audioManager.FadeMinigameAudio();

        SceneManager.UnloadSceneAsync(minigameStatus.nextMinigame.sceneName);

        UpdateMinigameStatus();
        RunIntermission(minigameStatus);

        minigameEndCoroutine = null;
    }

    private void UpdateMinigameStatus()
    {
        // evalutate result
        minigameStatus.previousMinigame = minigameStatus.nextMinigame;
        minigameStatus.previousMinigameResult = isCurrentMinigameWon ? WinLose.WIN : WinLose.LOSE;

        if (isCurrentMinigameWon)
        {
            //animations
            currProgressBar += upgradeManager.CalcDamage();
        }
        else
        {
            //animations
            encounterHealth -= upgradeManager.CalcHealthLost(10f);
            //flash animations?
        }

        UpdateEncounterUI();

        if (currProgressBar == tgtProgressBar)
        {
            EndEncounter(false);
        }
        if (encounterHealth <= 0)
        {
            minigameStatus.gameResult = WinLose.LOSE;
            lives--;
            EndEncounter(true);
        }
        else if (currProgressBar >= tgtProgressBar)
        {
            minigameStatus.gameResult = WinLose.WIN;
            upgradeManager.DoUpgrade(LoadNextEncounter);
            EndEncounter(false);
        }
        else
        {
            minigameStatus.gameResult = WinLose.NONE;
            // game still running, proceed with next round
            minigameStatus.nextMinigame = minigamePool[(minigameIndex + UnityEngine.Random.Range(1, minigamePool.Count)) % minigamePool.Count];
            Managers.__instance.scenesManager.LoadMinigameScene(minigameStatus.nextMinigame);
        }

    }

    private void EndEncounter(bool onLose)
    {
        if (lives == 0 && onLose)
        {
            Managers.__instance.scenesManager.LoadSceneImmediate("End");
        }
        LoadNextEncounter();
    }

    public void RunIntermission(MinigameStatus status)
    {
        if (OnBeginIntermission == null)
        {
            Debug.LogWarning("No one is subscribed to OnBeginIntermission. This is probably a mistake because we expect a listener here to then later call LoadNextMinigame");
        }
        UpdatePlayerStatsUI();
        if (status.gameResult == WinLose.NONE)
        {
            OnBeginIntermission?.Invoke(status, StartNextMinigame);
        }

    }


    // Called when all of the between-minigame cinematics are complete and the
    // next minigame is ready to be put on screen.
    public void StartNextMinigame()
    {
        if (isMinigamePlaying)
        {
            Debug.LogError("Cannot load next minigame when a minigame is playing!");
            return;
        }

        // we set these now even though the minigame scene might not be loaded because if we wait
        // until after they are loaded, these may overwrite Awake() and Start() calls in minigame scripts
        isMinigamePlaying = true;
        isCurrentMinigameWon = false;

        Managers.__instance.audioManager.StartMinigameAudio();
        Managers.__instance.scenesManager.ActivateMinigameScene(() =>
        {
            OnStartMinigame?.Invoke(minigameStatus.nextMinigame);
        });
    }


    public MinigameDefinition GetMinigameDefForScene(Scene scene)
    {
        return allMinigames.Find(mDef => mDef.sceneName == scene.name);
    }

    //TODO
    private void LoadNextEncounter()
    {
        //call choicer
        //encounterStatus = choicer
        //use dataTransfer to update
        //reloadscence with updated stats
        Managers.__instance.scenesManager.LoadSceneImmediate("Main");

    }

    private void UpdatePlayerStatsUI()
    {
        showCritChance.text = $"Crit. Chance: {critChance}%";
        showDamage.text = $"Damage: {damage}%";

    }

    private void UpdateEncounterUI()
    {
        progSlider.maxValue = tgtProgressBar;
        healthSlider.maxValue = maxHealth;
        progSlider.value = currProgressBar;
        healthSlider.value = encounterHealth;
    }
}
