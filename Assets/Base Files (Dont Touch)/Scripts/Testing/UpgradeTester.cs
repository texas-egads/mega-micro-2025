using TMPro;
using UnityEngine;

public class UpgradeTester : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI text2;
    [SerializeField] private AnimationCurve encounterHealth;
    [SerializeField] private AnimationCurve encounterDamage;
    private UpgradeManager uMan
    {
        get
        {
            return Managers.__instance.upgradeManager;
        }
    }
    private MinigamesManager mMan
    {
        get
        {
            return Managers.__instance.minigamesManager;
        }
    }
    private EncounterManager eMan
    {
        get
        {
            return Managers.__instance.encounterManager;
        }
    }
    private int round;
    private float currentHealth;
    private int status;
    private string status1;
    private string status2;
    private UpgradeManager.EncounterType roundType;
    private float lastDamage;
    private float lastHurt;
    private bool phase2;
    private bool waitingForEncounterChoice;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        round = 1;
    }

    // Update is called once per frame
    void Update()
    {
        // Stop when done
        if (status != 0) return;

        if (waitingForEncounterChoice) return;

        // Setup
        if (currentHealth == 0)
        {
            waitingForEncounterChoice = true;

            uMan.EncounterStart();
            mMan.health = uMan.Health;

            // Start Encounter Choicer
            eMan.StartEncounterChoicer(round, (firstEncounter) =>
            {
                currentHealth = firstEncounter.health;
                roundType = firstEncounter.type;

                status1 = "N/A";
                status2 = "N/A";

                waitingForEncounterChoice = false;
            });

            return;
        }

        int autoPick = 0;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            autoPick = mMan.GetCurrentMinigameDifficulty() <= Random.Range(-.25f, 1.75f) ? 2 : 1;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) || autoPick == 1)
        {
            // Lose microgame
            status1 = "Lost";
            lastHurt = uMan.CalcHealthLost(encounterDamage.Evaluate(round));
            mMan.encounterHealth -= lastHurt;
            if (mMan.encounterHealth <= 0)
            {
                status2 = "Lost";
                // Lose encounter
                mMan.lives--;
                if (mMan.lives <= 0)
                {
                    // Lose overall
                    status = -1;
                }
                else
                {
                    PostUpgrade();
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || autoPick == 2)
        {
            // Win microgame
            status1 = "Won";
            lastDamage = uMan.CalcDamage();
            currentHealth -= lastDamage;
            if (round == 15 && !phase2 && currentHealth <= encounterHealth.Evaluate(round))
            {
                phase2 = true;
            }
            if (phase2)
            {
                currentHealth = Mathf.Clamp(currentHealth + encounterHealth.Evaluate(round) * 0.4f, 0, encounterHealth.Evaluate(round) * 2);
            }
            if (currentHealth <= 0)
            {
                status2 = "Won";
                // Win encounter
                round++;
                if (round == 16)
                {
                    // Win overall
                    status = 1;
                }
                else
                {
                    Managers.__instance.upgradeManager.DoUpgrade(PostUpgrade);
                }
            }
        }

        text2.text = $"Round {round}\nType {roundType}\nHealth {currentHealth}\nDamage {encounterDamage.Evaluate(round)}\nLast Attack {lastDamage}\nLast Damage Taken {lastHurt}\nMicrogame {status1}\nEncounter {status2}\nGame Status ";
        if (status == 0)
        {
            text2.text += "In Progress";
        }
        else if (status == -1)
        {
            text2.text += "Lost";
        }
        else
        {
            text2.text += "Won";
        }
        text2.text += $"\n\nPlayer:\nDifficulty {mMan.GetCurrentMinigameDifficulty()}\nDamage {uMan.Damage}\nHealth {mMan.encounterHealth}\nCrit {uMan.CritChance}";
    }

    void PostUpgrade()
    {
        text.text = Managers.__instance.upgradeManager?.GetText();

        waitingForEncounterChoice = true;

        eMan.StartEncounterChoicer(round, (encounter) =>
        {
            uMan.EncounterStart(encounter.type);

            status1 = "N/A";
            mMan.health = uMan.Health;

            roundType = encounter.type;
            currentHealth = encounter.health * (roundType == UpgradeManager.EncounterType.BOSS ? 2 : 1);

            waitingForEncounterChoice = false;
        });
    }
}
