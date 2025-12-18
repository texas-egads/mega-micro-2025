using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MinigameStatus
{
    public WinLose previousMinigameResult;
    public MinigameDefinition previousMinigame;
    public MinigameDefinition nextMinigame;
    public WinLose gameResult;
}

public enum WinLose {
    NONE, // if the thing is not a win or a loss - could be in progress, or undefined
    WIN,
    LOSE
}
