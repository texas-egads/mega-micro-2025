using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{

    // Meta 
    private float lives;
    private float health
    {
        get
        {
            return Managers.__instance.minigamesManager.encounterHealth;
        }
    }
    public enum EncounterType
    {
        NORMAL,
        ELITE,
        BOSS
    }
    private EncounterType encounterType;
    private float difficulty
    {
        get
        {
            return Managers.__instance.minigamesManager.minigameDifficulty;
        }
        set
        {
            Managers.__instance.minigamesManager.minigameDifficulty = value;
        }
    }

    // Base stats
    [SerializeField] private float baseDamage;
    private float flatDamageIncrease;
    public float Damage
    {
        get
        {
            return CalcDamageRaw();
        }
    }
    public float Health
    {
        get
        {
            return CalcInitialHealth();
        }
    }
    [SerializeField] private float baseHealth;
    private float damageResistance;
    [SerializeField] private List<float> difficultyAdjustments;
    private float nulledElites;
    [SerializeField] private float eliteDifficultyIncrease;
    [SerializeField] private float[] difficultyGambleRange;
    public float CritChance;
    [SerializeField] private float critDamage;

    // Special upgrades
    private int specialTraits;
    [SerializeField] private float damageMultByDifficulty;
    const int MASK_DamageMultByDifficulty = 1 << 0;
    [SerializeField] private float eliteDamageMult;
    const int MASK_EliteDamageMult = 1 << 1;
    [SerializeField] private float bossDamageMult;
    const int MASK_BossDamageMult = 1 << 2;
    [SerializeField] private float healthDamageScalingMinor;
    const int MASK_HealthDamageScalingMinor = 1 << 3;
    [SerializeField] private float healthDamageScalingMajor;
    const int MASK_HealthDamageScalingMajor = 1 << 4;
    [SerializeField] private float difficultyDamageScaling;
    const int MASK_DifficultyDamageScaling = 1 << 5;
    [SerializeField] private float lifeDamageScaling;
    const int MASK_LifeDamageScaling = 1 << 6;
    [SerializeField] private float damageVariance;
    const int MASK_DamageVariance = 1 << 7;
    [SerializeField] private float difficultyHealthScaling;
    const int MASK_DifficultyHealthScaling = 1 << 8;
    [SerializeField] private float regenParams;
    const int MASK_Regeneration = 1 << 9;

    // Upgrade meta stats
    [SerializeField] private float[] rarityOdds;
    private int[] upgradeTypes;
    [SerializeField] private int baseRarityBias;
    [SerializeField] private GameObject[] damageUpgrades;
    [SerializeField] private GameObject[] healthUpgrades;
    [SerializeField] private GameObject[] difficultyUpgrades;
    [SerializeField] private GameObject[] critUpgrades;
    public void Initialize()
    {
        GameObject newObj = new GameObject();
        newObj.transform.parent = transform;
        difficultyAdjustments = new List<float>();
        for (int i = 0; i < 5; i++)
        {
            difficultyAdjustments.Add(0);
        }
        upgradeTypes = new int[4];
        for (int i = 0; i < 4; i++)
        {
            upgradeTypes[i] = baseRarityBias;
        }
    }
    /** INTERFACE METHODS**/
    // Sets up encounter stats
    public void EncounterStart(int currentLives, int type = 0)
    {
        lives = currentLives;
        encounterType = (EncounterType)type;
    }
    // Picks upgrades and instantiates their game objects
    public void DoUpgrade()
    {
        //TODO
    }
    // Determines damage of an attack
    public float CalcDamage()
    {
        //TODO
        return 0f;
    }
    // Determines damage taken from an attack
    public float CalcDamageTaken()
    {
        //TODO
        return 0f;
    }

    /** INTERNAL METHODS**/
    private float CalcDamageRaw()
    {
        bool doEliteScalar = (MASK_EliteDamageMult & specialTraits) > 0;
        float eliteScalar = (encounterType == EncounterType.ELITE && doEliteScalar) ? eliteDamageMult - 1 : 0;
        bool doBossScalar = (MASK_BossDamageMult & specialTraits) > 0;
        float bossScalar = (encounterType == EncounterType.BOSS && doBossScalar) ? bossDamageMult - 1 : 0;
        float difficultyScalar = ((MASK_DamageMultByDifficulty & specialTraits) > 0) ? difficulty * difficultyHealthScaling + 0.5f : 0;
        float healthMinorScalar = ((MASK_HealthDamageScalingMinor & specialTraits) > 0) ? health * healthDamageScalingMinor : 0;
        float healthMajorScalar = ((MASK_HealthDamageScalingMajor & specialTraits) > 0) ? health * healthDamageScalingMajor : 0;
        //[SerializeField] private float difficultyDamageScaling;
        const int MASK_DifficultyDamageScaling = 1 << 5;
        //[SerializeField] private float lifeDamageScaling;
        const int MASK_LifeDamageScaling = 1 << 6;
        return (baseDamage + flatDamageIncrease) * eliteScalar;
    }
    private float CalcInitialHealth()
    {
        float difficultyScalar = ((MASK_DifficultyHealthScaling & specialTraits) > 0) ? 1 + (difficulty * difficultyHealthScaling) : 1;
        return baseHealth * difficultyScalar;
    }

}
