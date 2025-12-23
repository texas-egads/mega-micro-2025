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
            return Managers.__instance.minigamesManager.GetCurrentMinigameDifficulty();
        }
        set
        {
            Managers.__instance.minigamesManager.minigameDifficulty = value;
        }
    }
    private float rounds;

    // Base stats
    [SerializeField] private float difficultyScaling;
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
    private List<float> difficultyAdjustments;
    private int nulledElites;
    [SerializeField] private float eliteDifficultyIncrease;
    public float CritChance;
    [SerializeField] private float critDamage;

    // Special upgrades
    private int specialTraits;
    private float eliteDamageMult;
    const int MASK_EliteDamageMult = 1 << 1;
    private float bossDamageMult;
    const int MASK_BossDamageMult = 1 << 2;
    private float healthDamageScalingMinor;
    const int MASK_HealthDamageScalingMinor = 1 << 3;
    private float healthDamageScalingMajor;
    const int MASK_HealthDamageScalingMajor = 1 << 4;
    private float difficultyDamageScaling;
    const int MASK_DifficultyDamageScaling = 1 << 5;
    private float lifeDamageScaling;
    const int MASK_LifeDamageScaling = 1 << 6;
    private float damageVariance;
    const int MASK_DamageVariance = 1 << 7;
    private float difficultyHealthScaling;
    const int MASK_DifficultyHealthScaling = 1 << 8;
    private float regenTime;
    private float regenTimer;
    const int MASK_Regeneration = 1 << 9;

    // Upgrade meta stats
    [SerializeField] private float[] rarityOdds;
    private int[] upgradeTypesTaken;
    [SerializeField] private GameObject[] upgradeList;
    private List<List<GameObject>>[] upgrades;
    private List<GameObject> upgradeChoices;
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
    [SerializeField] private GameObject upgradeScreen;
    private Dictionary<Upgrade.UpgradeName, System.Action> upgradeMethods;
    private System.Action returnFunc;
    public void Initialize()
    {
        GameObject newObj = new GameObject();
        newObj.transform.parent = transform;
        // Setup lists
        difficultyAdjustments = new List<float>();
        upgradeChoices = new List<GameObject>();
        for (int i = 0; i < 10; i++)
        {
            difficultyAdjustments.Add(0);
        }
        upgradeTypesTaken = new int[4];
        upgrades = new List<List<GameObject>>[4];
        prevUpgrades = new List<Upgrade.UpgradeName>();
        damageUpgrades = new List<List<GameObject>>();
        healthUpgrades = new List<List<GameObject>>();
        difficultyUpgrades = new List<List<GameObject>>();
        critUpgrades = new List<List<GameObject>>();
        for (int i = 0; i < 4; i++)
        {
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
            if (!u)
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
        upgradeMethods.Add(Upgrade.UpgradeName.DAMAGE_VARIANCE, UpgradeDamageVariance);
        upgradeMethods.Add(Upgrade.UpgradeName.GAMBLE_UPGRADES_SET, UpgradeGambleUpgradesSet);
    }
    /** INTERFACE METHODS **/
    // Sets up encounter stats
    private bool lostPrev;
    public void EncounterStart(EncounterType type = EncounterType.NORMAL)
    {
        encounterType = type;
        rounds = 0;
        if (lostPrev)
        {
            difficulty += difficultyScaling / 2f;
        }
        lostPrev = true;

        // Handle temp difficulty changes
        difficulty += difficultyAdjustments[0];
        difficultyAdjustments.RemoveAt(0);
        difficultyAdjustments.Add(0);

        // Handle elite difficulty changes
        if (encounterType == EncounterType.ELITE)
        {
            extraUpgrades++;
            if (nulledElites > 0)
            {
                nulledElites--;
            }
            else
            {
                difficulty += eliteDifficultyIncrease;
                difficultyAdjustments[0] -= eliteDifficultyIncrease;
            }
        }

        // Handle regen
        if ((MASK_Regeneration & specialTraits) > 0)
        {
            if (regenTimer <= 0)
            {
                lives++;
                regenTimer = regenTime;
            }
            regenTimer--;
        }
    }
    // Picks upgrades and instantiates their game objects
    public void DoUpgrade(System.Action returnAction)
    {
        // Set return
        returnFunc = returnAction;

        // Start display
        upgradeScreen.SetActive(true);

        // Pick Upgrades
        List<int> types = new List<int>();
        types.Add(0); types.Add(1); types.Add(2); types.Add(3);
        for (int i = 0; i < 2; i++)
        {
            int type = Random.Range(0, types.Count);
            List<GameObject> pool = upgrades[types[type]][GetRandomRarity()];
            upgradeChoices.Add(pool[Random.Range(0, pool.Count)]);
            types.RemoveAt(type);
        }
        int maxFreq = -1;
        int maxIndex = 0;
        foreach (int t in types)
        {
            if (upgradeTypesTaken[t] > maxFreq)
            {
                maxFreq = upgradeTypesTaken[t];
                maxIndex = t;
            }
        }
        List<GameObject> maxPool = upgrades[maxIndex][GetRandomRarity()];
        upgradeChoices.Add(maxPool[Random.Range(0, maxPool.Count)]);

        // Enable upgrades
        upgradeChoices[0].transform.SetAsFirstSibling();
        upgradeChoices[2].transform.SetAsLastSibling();
        upgradeChoices[0].SetActive(true);
        upgradeChoices[1].SetActive(true);
        upgradeChoices[2].SetActive(true);
    }
    // Determines damage of an attack
    public float CalcDamage()
    {
        rounds++;
        float res = CalcDamageRaw();
        if ((MASK_DamageVariance & specialTraits) > 0) res *= Random.Range(1 - damageVariance, 1 + damageVariance);
        if (Random.Range(0f, 1f) <= CritChance) res *= critDamage;
        return res;
    }
    // Determines damage taken from a failed minigame
    public float CalcHealthLost(float rawDamage)
    {
        rounds++;
        return rawDamage * Mathf.Clamp01(1 - damageResistance) + Health * ((rounds - 1) / 15f);
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
        float healthMinorAdder = ((MASK_HealthDamageScalingMinor & specialTraits) > 0) ? Mathf.Clamp(health - baseHealth, 0, health) * healthDamageScalingMinor : 0;
        float healthMajorAdder = ((MASK_HealthDamageScalingMajor & specialTraits) > 0) ? Mathf.Clamp(health - baseHealth, 0, health) * healthDamageScalingMajor : 0;
        float lifeScaling = ((MASK_LifeDamageScaling & specialTraits) > 0) ? Mathf.Clamp(lives - 3, 0, lives) * lifeDamageScaling : 0;
        return (baseDamage + flatDamageIncrease + healthMinorAdder + healthMajorAdder) * (1 + eliteScalar + bossScalar + difficultyScalar + lifeScaling);
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
        upgradeScreen.SetActive(false);
        upgradeChoices[0].SetActive(false);
        upgradeChoices[1].SetActive(false);
        upgradeChoices[2].SetActive(false);
        upgradeChoices.Clear();

        // Call appropriate method
        prevUpgrades.Add(u.id);
        activeUpgrade = u;
        upgradeMethods[u.id]();
        activeUpgrade = null;

        // Repeat if needed
        if (extraUpgrades > 0)
        {
            extraUpgrades--;
            DoUpgrade(returnFunc);
            return;
        }

        // Return control to manager
        difficulty += difficultyScaling;
        lostPrev = false;
        returnFunc();
    }
    private List<Upgrade.UpgradeName> prevUpgrades;
    public string GetText()
    {
        string res = "Upgrades Summary:"
            + $"\n\t Damage: {Damage}"
            + $"\n\t Health: {Health}"
            + $"\n\t Lives: {lives}"
            + $"\n\t Difficulty: {difficulty}"
            + $"\n\t Crit Chance: {CritChance}"
            + $"\n\t Crit Damage: {critDamage}"
            + $"\n\t damage bonuses: {flatDamageIncrease}"
            + $"\n\t elite damage: {(MASK_EliteDamageMult & specialTraits) > 0}"
            + $"\n\t difficulty damage: {(MASK_DifficultyDamageScaling & specialTraits) > 0}"
            + $"\n\t boss damage: {(MASK_BossDamageMult & specialTraits) > 0}"
            + $"\n\t health bonuses: {healthIncrease}"
            + $"\n\t health damage minor: {(MASK_HealthDamageScalingMinor & specialTraits) > 0}"
            + $"\n\t health damage major: {(MASK_HealthDamageScalingMajor & specialTraits) > 0}"
            + $"\n\t lives damage: {(MASK_LifeDamageScaling & specialTraits) > 0}"
            + $"\n\t damage resist: {damageResistance}"
            + $"\n\t difficulty health: {(MASK_DifficultyHealthScaling & specialTraits) > 0}"
            + $"\n\t regen: {(MASK_Regeneration & specialTraits) > 0}"
            + $"\n\t nulled elites: {nulledElites}"
            + $"\n\t elite difficulty: {difficulty + eliteDifficultyIncrease}";
        res += "\n\tdifficulty list(s):";
        for (int i = 0; i < difficultyAdjustments.Count; i++)
        {
            res += $" {difficultyAdjustments[i]}";
        }
        res += "\nlast upgrade(s): ";
        for (int i = 0; i < prevUpgrades.Count; i++)
        {
            res += $"\n\t{i}: {prevUpgrades[i]}";
        }
        return res;
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
        eliteDamageMult = activeUpgrade.val;
        specialTraits |= MASK_EliteDamageMult;
        damageUpgrades[2].Remove(activeUpgrade.gameObject);
    }
    private void UpgradeDifficultyDamage()
    {
        upgradeTypesTaken[0]++;
        difficultyDamageScaling = activeUpgrade.val;
        specialTraits |= MASK_DifficultyDamageScaling;
        damageUpgrades[3].Remove(activeUpgrade.gameObject);
    }
    private void UpgradeBossDamage()
    {
        upgradeTypesTaken[0]++;
        bossDamageMult = activeUpgrade.val;
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
        healthDamageScalingMinor = activeUpgrade.val;
        specialTraits |= MASK_HealthDamageScalingMinor;
        healthUpgrades[1].Remove(activeUpgrade.gameObject);
    }
    private void UpgradeHealthDamageMajor()
    {
        upgradeTypesTaken[1]++;
        healthDamageScalingMajor = activeUpgrade.val;
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
        lifeDamageScaling = activeUpgrade.val;
        specialTraits |= MASK_LifeDamageScaling;
        healthUpgrades[2].Remove(activeUpgrade.gameObject);
    }
    private void UpgradeDifficultyHealth()
    {
        upgradeTypesTaken[1]++;
        difficultyHealthScaling = activeUpgrade.val;
        specialTraits |= MASK_DifficultyHealthScaling;
        healthUpgrades[3].Remove(activeUpgrade.gameObject);
    }
    private void UpgradeRegeneration()
    {
        upgradeTypesTaken[1]++;
        regenTime = activeUpgrade.val;
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
        difficulty += Mathf.Round(Random.Range(activeUpgrade.val, activeUpgrade.val2) * 20) / 20;
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
        if (Random.Range(0f, 1f) < activeUpgrade.val2)
        {
            lives = Mathf.Clamp(lives - 1, 1, lives);
        }
        critUpgrades[2].Remove(activeUpgrade.gameObject);
    }
    private void UpgradeDamageVariance()
    {
        upgradeTypesTaken[3]++;
        damageVariance = activeUpgrade.val;
        specialTraits |= MASK_DamageVariance;
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
