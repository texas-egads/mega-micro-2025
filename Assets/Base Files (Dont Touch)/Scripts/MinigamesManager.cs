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
    [SerializeField] private List<MinigameDefinition> allMinigames;
    [SerializeField] private List<MinigameDefinition> timingMinigames;
    [SerializeField] private List<MinigameDefinition> precisionMinigames;
    [SerializeField] private List<MinigameDefinition> spamMinigames;
    [SerializeField] private List<MinigameDefinition> movementMinigames;
    [SerializeField] private GameObject[] containers;

    public Action<MinigameStatus, Action> OnBeginIntermission;
    public Action<MinigameDefinition> OnStartMinigame;
    public Action OnEndMinigame;

    public float encounterHealth;
    public float maxHealth;
    public float currProgressBar;
    public float tgtProgressBar;
    public int lives;

    //encounter stats
    //encounter type use not determined
    private UpgradeManager.EncounterType encounterType;
    private float critChance;
    private float damage;
    public int encounterNum;

    //Stats UI
    public GameObject playerStats;
    public TextMeshProUGUI showDamage;
    public TextMeshProUGUI showCritChance;
    public TextMeshProUGUI showEncounter;
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
    private Encounter currentEncounter;

    private bool isMinigamePlaying;
    private bool isCurrentMinigameWon;
    public bool gameWon
    {
        get
        {
            return isCurrentMinigameWon;
        }
    }
    private Coroutine minigameEndCoroutine;
    private int round;
    //TODO: Organize variable names for consistency. Tests gameplay loop & implement UI.
    public void Initialize()
    {
        isMinigamePlaying = false;
        isCurrentMinigameWon = false;
        lives = 3;
        minigameDifficulty = 0.0f; //Place Holder
    }

    public void StartMinigames()
    {
        UpdatePlayerStatsUI();
        Managers.__instance.encounterManager.StartEncounterChoicer(round, (currentEncounter) =>
        {
            // Set data
            this.currentEncounter = currentEncounter;
            encounterType = currentEncounter.type;
            upgradeManager.EncounterStart(encounterType);
            round++;
            maxHealth = upgradeManager.Health;
            critChance = upgradeManager.CritChance;
            damage = upgradeManager.Damage;
            encounterHealth = maxHealth;
            tgtProgressBar = currentEncounter.tgtProgress;
            currProgressBar = 0;

            //select kind of minigame
            if (currentEncounter.minigameType == Encounter.MinigameType.SPAM)
            {
                minigamePool = spamMinigames;
            }
            else if (currentEncounter.minigameType == Encounter.MinigameType.PRECISION)
            {
                minigamePool = timingMinigames;
            }
            else
            {
                minigamePool = allMinigames;
            }
            minigameIndex = UnityEngine.Random.Range(0, minigamePool.Count);
            minigameStatus.gameResult = WinLose.NONE;
            minigameStatus.nextMinigame = minigamePool[minigameIndex];
            Managers.__instance.scenesManager.LoadMinigameScene(minigameStatus.nextMinigame);
            RunIntermission(minigameStatus);
        }
        );
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
            return;
        }
        minigameEndCoroutine = StartCoroutine(DoEndMinigame(0));
    }

    private IEnumerator DoEndMinigame(float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        isMinigamePlaying = false;
        foreach (GameObject g in containers)
        {
            g.SetActive(true);
        }
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
            encounterHealth -= upgradeManager.CalcHealthLost(currentEncounter.failedPunishment);
            //flash animations?
        }

        UpdateEncounterUI();

        if (encounterHealth <= 0)
        {
            minigameStatus.gameResult = WinLose.LOSE;
            lives--;
            EndEncounter();
        }
        else if (currProgressBar >= tgtProgressBar)
        {
            if (lives > 0)
            {
                minigameStatus.gameResult = WinLose.WIN;
                upgradeManager.DoUpgrade(EndEncounter);
            }
            else
            {
                EndEncounter();
            }
        }

        else
        {
            minigameStatus.gameResult = WinLose.NONE;
            // game still running, proceed with next round
            minigameStatus.nextMinigame = minigamePool[(minigameIndex + UnityEngine.Random.Range(1, minigamePool.Count)) % minigamePool.Count];
            Managers.__instance.scenesManager.LoadMinigameScene(minigameStatus.nextMinigame);
        }

    }

    private void EndEncounter()
    {
        if (lives == 0)
        {
            Debug.Log("Ending encounter");
            playerStats.SetActive(false);
            if (Managers.__instance)
            {
                Managers.__instance.scenesManager.LoadSceneImmediate("End");
            }
            else
            {
                Debug.Log("failed to end");
            }
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

        foreach (GameObject g in containers)
        {
            g.SetActive(false);
        }
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

    private void LoadNextEncounter()
    {
        Managers.__instance.scenesManager.LoadSceneImmediate("Main");
    }

    private void UpdatePlayerStatsUI()
    {
        if (showCritChance) showCritChance.text = $"Crit. Chance: {critChance}%";
        if (showDamage) showDamage.text = $"Production: {damage}";
        if (showEncounter) showEncounter.text = $"Encounter#: {encounterNum}";
    }

    private void UpdateEncounterUI()
    {
        progSlider.maxValue = tgtProgressBar;
        healthSlider.maxValue = maxHealth;
        progSlider.value = currProgressBar;
        healthSlider.value = encounterHealth;
    }
}
