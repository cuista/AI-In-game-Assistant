using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent : MonoBehaviour
{
    public const string GAMEOVER = "GAMEOVER";
    public const string CUTSCENE_STARTED = "CUTSCENE_STARTED";
    public const string CUTSCENE_ENDED = "CUTSCENE_ENDED";
    public const string CUTSCENE_STOPPED = "CUTSCENE_STOPPED";
    public const string SPEED_CHANGED = "SPEED_CHANGED";

    public static bool isPaused = false;
}
