using UnityEngine;

public class Encounter
{
    public enum MinigameType
    {
        ALL,
        SPAM,
        PRECISION,
        TIMING,
        MOVEMENT
    }

    public UpgradeManager.EncounterType type;
    public MinigameType minigameType;
    public float failedPunishment; //health point deduction from failing one minigame
    public int tgtProgress; //target progress to be reached this encounter
    public EncounterManager.Flavors flavor;
}
