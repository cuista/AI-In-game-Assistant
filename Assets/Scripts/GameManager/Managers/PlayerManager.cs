using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    public int health { get; private set; }
    public int maxHealth { get; private set; }
    public int PotionBig { get; private set; }
    public int PotionSmall { get; private set; }
    public int barValueDamage { get; private set; }

    public void Startup()
    {
        Debug.Log("Player manager starting...");
        health = 10;
        maxHealth = 100;
        PotionBig = 1;
        PotionSmall = 5;
        barValueDamage = maxHealth / health;
        status = ManagerStatus.Started;
    }
}
