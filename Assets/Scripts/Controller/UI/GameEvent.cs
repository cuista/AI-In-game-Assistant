using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent : MonoBehaviour
{
    public const string ENEMY_KILLED = "ENEMY_KILLED";
    public const string GAMEOVER = "GAMEOVER";
    public const string CUTSCENE_STARTED = "CUTSCENE_STARTED";
    public const string CUTSCENE_ENDED = "CUTSCENE_ENDED";
    public const string CUTSCENE_STOPPED = "CUTSCENE_STOPPED";
    public const string SPEED_CHANGED = "SPEED_CHANGED";
    public const string TARGET_TOTAL = "TARGET_TOTAL";
    public const string TARGET_ELIMINATED = "TARGET_ELIMINATED";

    public static bool isPaused = false;
}
