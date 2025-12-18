using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public enum UpgradeType
    {
        DAMAGE,
        HEALTH,
        DIFFICULTY,
        CRIT
    }
    public enum UpgradeRarity
    {
        RARE,
        SUPERRARE,
        EPIC,
        LEGENDARY
    }
    public enum UpgradeName
    {
        DAMAGE_FLAT,
        DAMAGE_MULT,
        ELITE_DAMAGE,
        DIFFICULTY_DAMAGE,
        BOSS_DAMAGE,
        HEALTH,
        HEALTH_DAMAGE_MINOR,
        HEALTH_DAMAGE_MAJOR,
        DAMAGE_RESISTANCE,
        LIVES,
        DAMAGE_LIVES,
        DIFFICULTY_HEALTH,
        REGENERATION,
        DIFFICULTY,
        DIFFICULTY_UNCAPPED,
        NULL_ELITE,
        LIKELIER_ELITES,
        EASIER_ELITES,
        GAMBLE_DIFFICULTY,
        CRIT,
        GAMBLE_UPGRADES_FLAT,
        GAMBLE_UPGRADES_SET
    }
    public UpgradeType type;
    public UpgradeRarity rarity;
    public UpgradeName id;
    public float val;
    public float val2;
}
