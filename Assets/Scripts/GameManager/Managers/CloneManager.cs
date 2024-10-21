using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    public int health { get; private set; }
    public int maxHealth { get; private set; }
    public int barValueDamage { get; private set; }

    public void Startup()
    {
        Debug.Log("Clone manager starting...");
        health = 10;
        maxHealth = 100;
        barValueDamage = maxHealth / health;
        status = ManagerStatus.Started;
    }
}
