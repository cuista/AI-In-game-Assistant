using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    void RemoveLives(int livesToRemove);

    int GetLives();

    bool IsMoving();

    void SetMoving(bool moving);

    void OnSpeedChanged(float value);
}
