using UnityEngine;

public class Encounter
{
    public UpgradeManager.EncounterType type;
    public float failedPunishment; //health point deduction from failing one minigame
    public int tgtProgress; //target progress to be reached this encounter
    public EncounterManager.Flavors flavor;
}
