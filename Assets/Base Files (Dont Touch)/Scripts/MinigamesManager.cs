using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public float health;
    public int lives;

    int minigameIndex;
    List<MinigameDefinition> minigamePool;

    private UpgradeManager upgradeManager
    {
        get
        {
            return Managers.__instance.minigamesManager.upgradeManager;
        }
    }

    public float minigameDifficulty;

    private MinigameStatus status;

    private bool isMinigamePlaying;
    private bool isCurrentMinigameWon;

    // stores a preloaded scene for an upcoming round.
    private AsyncOperation nextMinigameLoadOperation;

    private Coroutine minigameEndCoroutine;

    public void Initialize()
    {
        isMinigamePlaying = false;
        isCurrentMinigameWon = false;
    }

    public void StartMinigames()
    {
        //set health/maxhealth to upgradeManager.Health;

        //Call upgradeManager.StartEncounter();

        //Update stats UI accordingly

        //Update minigame pool/index appropriately

        status.nextMinigame = minigamePool[minigameIndex];
        Managers.__instance.scenesManager.LoadMinigameScene(status.nextMinigame);

        RunIntermission(status);
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
        return minigameDifficulty;
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

        SceneManager.UnloadSceneAsync(status.nextMinigame.sceneName);

        UpdateMinigameStatus();
        RunIntermission(status);

        minigameEndCoroutine = null;
    }

    private void UpdateMinigameStatus()
    {
        // evalutate result
        status.previousMinigame = status.nextMinigame;
        status.previousMinigameResult = isCurrentMinigameWon ? WinLose.WIN : WinLose.LOSE;

        if (isCurrentMinigameWon)
        {
            //animations
            //deal damage using upgradeManager.CalcDamage();
        }
        else
        {
            //animations
            //take damage using upgradeManager.CalcDamageTaken();
            //update healthbar/refresh stats ui
        }

        if (/*out of health*/)
        {
            status.gameResult = WinLose.LOSE;
            //decrement lives
            //end encounter
        }
        else if (/*encounter health done*/)
        {
            status.gameResult = WinLose.WIN;
            //DoUpgrade
        }
        else
        {
            status.gameResult = WinLose.NONE;
            // game still running, proceed with next round
            status.nextMinigame = minigamePool[(minigameIndex + UnityEngine.Random.Range(1, minigamePool.Count)) % minigamePool.Count];
            Managers.__instance.scenesManager.LoadMinigameScene(status.nextMinigame);
        }

    }

    private void EndEncounter()
    {
        //call encounter choicer
        //load next encounter and go to it and call start minigame again
    }

    public void PostUpgrade()
    {
        EndEncounter();
    }

    public void RunIntermission(MinigameStatus status)
    {
        if (OnBeginIntermission == null)
        {
            Debug.LogWarning("No one is subscribed to OnBeginIntermission. This is probably a mistake because we expect a listener here to then later call LoadNextMinigame");
        }

        if (status.gameResult == WinLose.NONE)
        {
            OnBeginIntermission?.Invoke(status, StartNextMinigame);
        }

    }


    // Called when all of the between-minigame cinematics are complete and the
    // next minigame is ready to be put on screen.
    public void StartNextMinigame()
    {

        //
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
        Managers.__instance.scenesManager.ActivateMinigameScene(() => {
            OnStartMinigame?.Invoke(status.nextMinigame);
        });
    }


    public MinigameDefinition GetMinigameDefForScene(Scene scene)
    {
        return allMinigames.Find(mDef => mDef.sceneName == scene.name);
    }
}
