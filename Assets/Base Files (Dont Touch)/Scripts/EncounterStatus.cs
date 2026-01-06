using UnityEngine;

public struct EncounterStatus
{
    public EncounterType encounterType;
    public bool isElite;
}

public enum EncounterType
{
    SKILL,
    TIME,
    RAND
}
