using DG.Tweening;
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
    [SerializeField] private List<MinigameDefinition> timingMinigames;
    [SerializeField] private List<MinigameDefinition> precisionMinigames;
    [SerializeField] private List<MinigameDefinition> spamMinigames;
    [SerializeField] private List<MinigameDefinition> movementMinigames;
    [SerializeField] private List<MinigameDefinition> forcedMinigames;
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
    public TextMeshProUGUI showDamage;
    public TextMeshProUGUI showCritChance;
    //public TextMeshProUGUI showEncounter;
    public Slider healthSlider;
    public Slider progSlider;
    public Image[] livesSprite = new Image[3];
    public Sprite deadLiveSprite;
    public TMP_Text healthText;

    public Animator healthUI;
    public Animator progressUI;
    public Animator statsUI;
    public Animator livesUI;

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

    private Coroutine minigameEndCoroutine;
    private int round;
    public MenuScreens winLoseMenu;
    private AudioClip winSound;
    public AudioClip loseSound;
    public void Initialize()
    {
        isMinigamePlaying = false;
        isCurrentMinigameWon = false;
        lives = 3;
        minigameDifficulty = PlayerPrefs.GetFloat("difficultyStart");
    }

    public void StartMinigames()
    {
        Managers.__instance.audioManager.music.volume = 0.5f;
        Managers.__instance.encounterManager.StartEncounterChoicer(round, (currentEncounter) =>
        {
            // Set data
            Managers.__instance.audioManager.music.volume = 1f;
            this.currentEncounter = currentEncounter;
            winSound = currentEncounter.winSound;
            encounterType = currentEncounter.type;
            upgradeManager.EncounterStart(encounterType);
            UpdatePlayerStatsUI();
            round++;
            // Set health/healthbars
            maxHealth = upgradeManager.Health;
            encounterHealth = maxHealth;
            oldHealth = maxHealth;
            healthSlider.maxValue = encounterHealth;
            healthSlider.value = encounterHealth;
            critChance = upgradeManager.CritChance;
            damage = upgradeManager.Damage;
            tgtProgressBar = currentEncounter.tgtProgress;
            currProgressBar = 0;
            oldProgressBar = 0;
            progSlider.maxValue = tgtProgressBar;
            progSlider.value = 0;
            healthText.text = Math.Round(encounterHealth).ToString();

            //select kind of minigame
            if (currentEncounter.minigameType == Encounter.MinigameType.SPAM)
            {
                minigamePool = spamMinigames;
            }
            else if (currentEncounter.minigameType == Encounter.MinigameType.PRECISION)
            {
                minigamePool = precisionMinigames;
            }
            else if (currentEncounter.minigameType == Encounter.MinigameType.TIMING)
            {
                minigamePool = timingMinigames;
            }
            else if (currentEncounter.minigameType == Encounter.MinigameType.MOVEMENT)
            {
                minigamePool = movementMinigames;
            }
            else
            {
                minigamePool = allMinigames;
            }
            if (forcedMinigames.Count > 0) minigamePool = forcedMinigames;
            minigameIndex = UnityEngine.Random.Range(0, minigamePool.Count);
            minigameStatus.gameResult = WinLose.NONE;
            minigameStatus.previousMinigame = null;
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
            currProgressBar = Mathf.Clamp(currProgressBar + upgradeManager.CalcDamage(), 0, tgtProgressBar);
            Managers.__instance.audioManager.PlaySFX(winSound, 1.5f);
        }
        else
        {
            //animations
            encounterHealth = Mathf.Clamp(encounterHealth - upgradeManager.CalcHealthLost(currentEncounter.failedPunishment), 0, maxHealth);
            Managers.__instance.audioManager.PlaySFX(loseSound, 1.5f);
        }

        UpdateEncounterUI();

        if (encounterHealth <= 0)
        {
            minigameStatus.nextMinigame = null;
            minigameStatus.gameResult = WinLose.LOSE;
        }
        else if (currProgressBar >= tgtProgressBar)
        {
            minigameStatus.nextMinigame = null;
            minigameStatus.gameResult = WinLose.WIN;
        }
        else
        {
            minigameStatus.gameResult = WinLose.NONE;
            // game still running, proceed with next round
            minigameIndex = (minigameIndex + UnityEngine.Random.Range(1, minigamePool.Count)) % minigamePool.Count;
            minigameStatus.nextMinigame = minigamePool[minigameIndex];

            Managers.__instance.scenesManager.LoadMinigameScene(minigameStatus.nextMinigame);
        }

    }

    private void EndEncounter(bool onLose)
    {
        if (lives == 0 && onLose)
        {
            winLoseMenu.transform.parent.gameObject.SetActive(true);
            winLoseMenu.ShowLoseScreen();
            return;
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
        OnBeginIntermission?.Invoke(status, StartNextMinigame);

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
        if (showCritChance) showCritChance.text = $"{Mathf.RoundToInt(critChance * 100)}%";
        if (showDamage) showDamage.text = $"{Mathf.RoundToInt(damage)}";
        //if (showEncounter) showEncounter.text = $"Encounter#: {encounterNum}";
    }

    private float oldHealth;
    private float oldProgressBar;
    private void UpdateEncounterUI()
    {
        progSlider.maxValue = tgtProgressBar;
        healthSlider.maxValue = maxHealth;
        oldProgressBar = progSlider.value;
        oldHealth = healthSlider.value;
        StartCoroutine("LerpSliders");
        healthText.text = Math.Round(encounterHealth).ToString();
    }

    IEnumerator LerpSliders()
    {
        float timer = 0f;
        while (timer <= 1f)
        {
            healthSlider.value = Mathf.SmoothStep(oldHealth, encounterHealth, timer);
            progSlider.value = Mathf.SmoothStep(oldProgressBar, currProgressBar, timer);
            timer += Time.deltaTime;
            yield return null;
        }
        if (encounterHealth <= 0)
        {
            Managers.__instance.audioManager.EndEncounter(false);
            yield return new WaitForSeconds(1.5f);
            lives--;
            UpdateLives();
            yield return new WaitForSeconds(2f);
            if (lives == 0) Managers.__instance.audioManager.FadeMusic();
            yield return new WaitForSeconds(0.5f);
            EndEncounter(true);
        }
        if (currProgressBar >= tgtProgressBar)
        {
            Managers.__instance.audioManager.EndEncounter(true);
            yield return new WaitForSeconds(1.5f);
            if (round == 15)
            {
                Managers.__instance.audioManager.FadeMusic();
            }
            yield return new WaitForSeconds(0.5f);
            if (round == 15)
            {
                winLoseMenu.transform.parent.gameObject.SetActive(true);
                winLoseMenu.ShowWinScreen();
            }
            else
            {
                upgradeManager.DoUpgrade(LoadNextEncounter);
            }
        }
    }

    private void UpdateLives()
    {
        if (lives < 3)
        {
            livesSprite[lives].sprite = deadLiveSprite;
        }

    }

    public void triggerUIExit()
    {
        healthUI.SetTrigger("out");
        progressUI.SetTrigger("out");
        statsUI.SetTrigger("out");
        livesUI.SetTrigger("out");
    }
}
