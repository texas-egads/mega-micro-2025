using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{

    // Meta 
    private int lives
    {
        get
        {
            return Managers.__instance.minigamesManager.lives;
        }
        set
        {
            Managers.__instance.minigamesManager.lives = value;
        }
    }
    private float health
    {
        get
        {
            return Managers.__instance.minigamesManager.health;
        }
    }
    private enum EncounterType
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
    private float healthIncrease;
    private float damageResistance;
    [SerializeField] private List<float> difficultyAdjustments;
    private int nulledElites;
    [SerializeField] private float eliteDifficultyIncrease;
    public float CritChance;
    [SerializeField] private float critDamage;

    // Special upgrades
    private int specialTraits;
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
    [SerializeField] private float regenTime;
    private float regenTimer;
    const int MASK_Regeneration = 1 << 9;

    // Upgrade meta stats
    [SerializeField] private float[] rarityOdds;
    private int[] upgradeTypesTaken;
    [SerializeField] private int baseTypeBias;
    [SerializeField] private GameObject[] upgradeList;
    private List<List<GameObject>>[] upgrades;
    private List<List<GameObject>> damageUpgrades
    {
        get { return upgrades[0]; }
        set { upgrades[0] = value; }
    }
    private List<List<GameObject>> healthUpgrades
    {
        get { return upgrades[1]; }
        set { upgrades[1] = value; }
    }
    private List<List<GameObject>> difficultyUpgrades
    {
        get { return upgrades[2]; }
        set { upgrades[2] = value; }
    }
    private List<List<GameObject>> critUpgrades
    {
        get { return upgrades[3]; }
        set { upgrades[3] = value; }
    }
    private int extraUpgrades;
    [SerializeField] private GameObject upgradeScreenPrefab;
    private GameObject upgradeScreen;
    private Dictionary<Upgrade.UpgradeName, System.Action> upgradeMethods;
    public void Initialize()
    {
        GameObject newObj = new GameObject();
        newObj.transform.parent = transform;
        // Setup lists
        difficultyAdjustments = new List<float>();
        for (int i = 0; i < 10; i++)
        {
            difficultyAdjustments.Add(0);
        }
        upgradeTypesTaken = new int[4];
        upgrades = new List<List<GameObject>>[4];
        damageUpgrades = new List<List<GameObject>>();
        healthUpgrades = new List<List<GameObject>>();
        difficultyUpgrades = new List<List<GameObject>>();
        critUpgrades = new List<List<GameObject>>();
        for (int i = 0; i < 4; i++)
        {
            upgradeTypesTaken[i] = baseTypeBias;
            damageUpgrades.Add(new List<GameObject>());
            healthUpgrades.Add(new List<GameObject>());
            difficultyUpgrades.Add(new List<GameObject>());
            critUpgrades.Add(new List<GameObject>());
        }
        // Sort upgrades
        foreach (GameObject g in upgradeList)
        {
            Upgrade u;
            g.TryGetComponent(out u);
            if(!u)
            {
                Debug.LogError("Gameobject lacks Upgrade component in upgrade list!");
            }
            if (u.type == Upgrade.UpgradeType.DAMAGE) damageUpgrades[(int)u.rarity].Add(g);
            if (u.type == Upgrade.UpgradeType.HEALTH) healthUpgrades[(int)u.rarity].Add(g);
            if (u.type == Upgrade.UpgradeType.DIFFICULTY) difficultyUpgrades[(int)u.rarity].Add(g);
            if (u.type == Upgrade.UpgradeType.CRIT) critUpgrades[(int)u.rarity].Add(g);
        }

        // Create upgrade mapping
        upgradeMethods = new Dictionary<Upgrade.UpgradeName, System.Action>();
        upgradeMethods.Add(Upgrade.UpgradeName.DAMAGE_FLAT, UpgradeDamageFlat);
        upgradeMethods.Add(Upgrade.UpgradeName.DAMAGE_MULT, UpgradeDamageMult);
        upgradeMethods.Add(Upgrade.UpgradeName.ELITE_DAMAGE, UpgradeEliteDamage);
        upgradeMethods.Add(Upgrade.UpgradeName.DIFFICULTY_DAMAGE, UpgradeDifficultyDamage);
        upgradeMethods.Add(Upgrade.UpgradeName.BOSS_DAMAGE, UpgradeBossDamage);
        upgradeMethods.Add(Upgrade.UpgradeName.HEALTH, UpgradeHealth);
        upgradeMethods.Add(Upgrade.UpgradeName.HEALTH_DAMAGE_MINOR, UpgradeHealthDamageMinor);
        upgradeMethods.Add(Upgrade.UpgradeName.HEALTH_DAMAGE_MAJOR, UpgradeHealthDamageMajor);
        upgradeMethods.Add(Upgrade.UpgradeName.DAMAGE_RESISTANCE, UpgradeDamageResistance);
        upgradeMethods.Add(Upgrade.UpgradeName.LIVES, UpgradeLives);
        upgradeMethods.Add(Upgrade.UpgradeName.DAMAGE_LIVES, UpgradeDamageLives);
        upgradeMethods.Add(Upgrade.UpgradeName.DIFFICULTY_HEALTH, UpgradeDifficultyHealth);
        upgradeMethods.Add(Upgrade.UpgradeName.REGENERATION, UpgradeRegeneration);
        upgradeMethods.Add(Upgrade.UpgradeName.DIFFICULTY, UpgradeDifficulty);
        upgradeMethods.Add(Upgrade.UpgradeName.DIFFICULTY_UNCAPPED, UpgradeDifficultyUncapped);
        upgradeMethods.Add(Upgrade.UpgradeName.NULL_ELITE, UpgradeNullElite);
        upgradeMethods.Add(Upgrade.UpgradeName.LIKELIER_ELITES, UpgradeLikelierElites);
        upgradeMethods.Add(Upgrade.UpgradeName.EASIER_ELITES, UpgradeEasierElites);
        upgradeMethods.Add(Upgrade.UpgradeName.GAMBLE_DIFFICULTY, UpgradeGambleDifficulty);
        upgradeMethods.Add(Upgrade.UpgradeName.CRIT, UpgradeCrit);
        upgradeMethods.Add(Upgrade.UpgradeName.GAMBLE_UPGRADES_FLAT, UpgradeGambleUpgradesFlat);
        upgradeMethods.Add(Upgrade.UpgradeName.GAMBLE_UPGRADES_SET, UpgradeGambleUpgradesSet);
    }
    /** INTERFACE METHODS **/
    // Sets up encounter stats
    public void EncounterStart(int type = 0)
    {
        encounterType = (EncounterType) type;

        // Handle elite difficulty changes
        if (encounterType == EncounterType.ELITE)
        {
            if(nulledElites > 0)
            {
                nulledElites--;
            } else
            {
                difficulty += eliteDifficultyIncrease;
            }
        }

        // Handle temp difficulty changes
        difficulty += difficultyAdjustments[0];
        difficultyAdjustments.RemoveAt(0);
        difficultyAdjustments.Add(0);

        // Handle regen
        if ((MASK_Regeneration & specialTraits) > 0)
        {
            if(regenTimer == 0)
            {
                lives++;
                regenTimer = regenTime;
            } else
            {
                regenTimer--;
            }
        }
    }
    // Picks upgrades and instantiates their game objects
    public void DoUpgrade()
    {
        // Start display
        GameObject container = Instantiate(upgradeScreenPrefab, GameObject.Find("Main Canvas").transform);

        // Pick Upgrades
        List<int> types = new List<int>();
        types.Add(0); types.Add(1); types.Add(2); types.Add(3);
        for (int i=0; i<2; i++)
        {
            int type = Random.Range(0, types.Count);
            List<GameObject> pool = upgrades[types[type]][GetRandomRarity()];
            Instantiate(pool[Random.Range(0, pool.Count)], container.transform);
            types.RemoveAt(type);
        }
        int maxFreq = 0;
        int maxIndex = 0;
        foreach (int t in types)
        {
            if(upgradeTypesTaken[t] > maxFreq)
            {
                maxFreq = upgradeTypesTaken[t];
                maxIndex = t;
            }
        }
        List<GameObject> maxPool = upgrades[maxIndex][GetRandomRarity()];
        Instantiate(maxPool[Random.Range(0, maxPool.Count)], container.transform);
    }
    // Determines damage of an attack
    public float CalcDamage()
    {
        float res = CalcDamageRaw();
        if ((MASK_DamageVariance & specialTraits) > 0) res *= Random.Range(1 - damageVariance, 1 + damageVariance);
        if (Random.Range(0f, 1f) <= CritChance) res *= critDamage;
        return res;
    }
    // Determines damage taken from an attack
    public float CalcDamageTaken(float rawDamage)
    {
        return rawDamage * Mathf.Clamp01(1-damageResistance);
    }

    /** INTERNAL METHODS **/
    private int GetRandomRarity()
    {
        float rarityVal = Random.Range(0f, 1f);
        for (int j = 0; j < 4; j++)
        {
            rarityVal -= rarityOdds[j];
            if (rarityVal <= 0)
            {
                return j;
            }
        }
        return 0;
    }
    private float CalcDamageRaw()
    {
        bool doEliteScalar = (MASK_EliteDamageMult & specialTraits) > 0;
        float eliteScalar = (encounterType == EncounterType.ELITE && doEliteScalar) ? eliteDamageMult - 1 : 0;
        bool doBossScalar = (MASK_BossDamageMult & specialTraits) > 0;
        float bossScalar = (encounterType == EncounterType.BOSS && doBossScalar) ? bossDamageMult - 1 : 0;
        float difficultyScalar = ((MASK_DifficultyDamageScaling & specialTraits) > 0) ? difficulty * difficultyDamageScaling : 0;
        float healthMinorScalar = ((MASK_HealthDamageScalingMinor & specialTraits) > 0) ? Mathf.Clamp(health - baseHealth, 0, health) * healthDamageScalingMinor : 0;
        float healthMajorScalar = ((MASK_HealthDamageScalingMajor & specialTraits) > 0) ? Mathf.Clamp(health - baseHealth, 0, health) * healthDamageScalingMajor : 0;
        float lifeScaling = ((MASK_LifeDamageScaling & specialTraits) > 0) ? Mathf.Clamp(lives - 3, 0, lives) * lifeDamageScaling : 0;
        return (baseDamage + flatDamageIncrease) * (1+eliteScalar+bossScalar+difficultyScalar+healthMajorScalar+healthMinorScalar+lifeScaling);
    }
    private float CalcInitialHealth()
    {
        float difficultyScalar = ((MASK_DifficultyHealthScaling & specialTraits) > 0) ? 1 + (difficulty * difficultyHealthScaling) : 1;
        return (baseHealth + healthIncrease) * difficultyScalar;
    }
    private Upgrade activeUpgrade;
    public void HandleUpgrade(Upgrade u)
    {
        // Hide options
        //TODO animate maybe?
        Destroy(upgradeScreen);
        upgradeScreen = null;

        // Call appropriate method
        activeUpgrade = u;
        upgradeMethods[u.id]();
        activeUpgrade = null;

        // Return control to manager
        Managers.__instance.minigamesManager.PostUpgrade();
    }

    /** UPGRADE METHODS **/
    //Damage
    private void UpgradeDamageFlat()
    {
        upgradeTypesTaken[0]++;
        flatDamageIncrease += activeUpgrade.val;
    }
    private void UpgradeDamageMult()
    {
        upgradeTypesTaken[0]++;
        flatDamageIncrease *= activeUpgrade.val;
    }
    private void UpgradeEliteDamage()
    {
        upgradeTypesTaken[0]++;
        specialTraits |= MASK_EliteDamageMult;
        damageUpgrades[2].Remove(activeUpgrade.gameObject);
    }
    private void UpgradeDifficultyDamage()
    {
        upgradeTypesTaken[0]++;
        specialTraits |= MASK_DifficultyDamageScaling;
        damageUpgrades[3].Remove(activeUpgrade.gameObject);
    }
    private void UpgradeBossDamage()
    {
        upgradeTypesTaken[0]++;
        specialTraits |= MASK_BossDamageMult;
        damageUpgrades[3].Remove(activeUpgrade.gameObject);
    }
    //health
    private void UpgradeHealth()
    {
        upgradeTypesTaken[1]++;
        healthIncrease += activeUpgrade.val;
    }
    private void UpgradeHealthDamageMinor()
    {
        upgradeTypesTaken[1]++;
        specialTraits |= MASK_HealthDamageScalingMinor;
        healthUpgrades[1].Remove(activeUpgrade.gameObject);
    }
    private void UpgradeHealthDamageMajor()
    {
        upgradeTypesTaken[1]++;
        specialTraits |= MASK_HealthDamageScalingMajor;
        healthUpgrades[2].Remove(activeUpgrade.gameObject);
    }
    private void UpgradeDamageResistance()
    {
        upgradeTypesTaken[1]++;
        damageResistance += activeUpgrade.val;
    }
    private void UpgradeLives()
    {
        upgradeTypesTaken[1]++;
        lives += (int)activeUpgrade.val;
    }
    private void UpgradeDamageLives()
    {
        upgradeTypesTaken[1]++;
        specialTraits |= MASK_LifeDamageScaling;
        healthUpgrades[2].Remove(activeUpgrade.gameObject);
    }
    private void UpgradeDifficultyHealth()
    {
        upgradeTypesTaken[1]++;
        specialTraits |= MASK_DifficultyHealthScaling;
        healthUpgrades[3].Remove(activeUpgrade.gameObject);
    }
    private void UpgradeRegeneration()
    {
        upgradeTypesTaken[1]++;
        specialTraits |= MASK_Regeneration;
        healthUpgrades[3].Remove(activeUpgrade.gameObject);
    }
    //Difficulty
    private void UpgradeDifficulty()
    {
        upgradeTypesTaken[2]++;
        difficulty -= activeUpgrade.val2;
        difficultyAdjustments[(int)activeUpgrade.val + 1] += activeUpgrade.val2;
    }
    private void UpgradeDifficultyUncapped()
    {
        upgradeTypesTaken[2]++;
        difficulty -= activeUpgrade.val;
    }
    private void UpgradeNullElite()
    {
        upgradeTypesTaken[2]++;
        nulledElites += (int)activeUpgrade.val;
    }
    private void UpgradeLikelierElites()
    {
        upgradeTypesTaken[2]++;
        //TODO
        difficultyUpgrades[1].Remove(activeUpgrade.gameObject);
    }
    private void UpgradeEasierElites()
    {
        upgradeTypesTaken[2]++;
        eliteDifficultyIncrease = activeUpgrade.val;
        difficultyUpgrades[1].Remove(activeUpgrade.gameObject);
    }
    private void UpgradeGambleDifficulty()
    {
        upgradeTypesTaken[2]++;
        difficulty += Random.Range(activeUpgrade.val, activeUpgrade.val2);
    }
    //Crit
    private void UpgradeCrit()
    {
        upgradeTypesTaken[3]++;
        CritChance += activeUpgrade.val;
        critDamage += activeUpgrade.val2;
    }
    private void UpgradeGambleUpgradesFlat()
    {
        upgradeTypesTaken[3]++;
        extraUpgrades += (int)activeUpgrade.val;
        if(Random.Range(0f, 1f) < activeUpgrade.val2)
        {
            lives = Mathf.Clamp(lives-1, 1, lives);
        }
        critUpgrades[2].Remove(activeUpgrade.gameObject);
    }
    private void UpgradeGambleUpgradesSet()
    {
        upgradeTypesTaken[3]++;
        extraUpgrades += (int)activeUpgrade.val;
        if (Random.Range(0f, 1f) < activeUpgrade.val2)
        {
            lives = 1;
        }
        critUpgrades[3].Remove(activeUpgrade.gameObject);
    }
}
