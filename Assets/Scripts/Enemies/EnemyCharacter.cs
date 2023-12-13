using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : MonoBehaviour, IEnemy, ICharacter
{
    [SerializeField] private int lives;

    private bool _isMoving;
    private bool _isShieldActive = false;

    private Vector3 _graviton;

    private void FixedUpdate()
    {
        if(GetLives() <= 0)
        {
            Death();
        }
    }

    public void Hurt(int damage)
    {
        RemoveLives(damage);
    }

    public void Death()
    {
        Destroy(this.gameObject);
    }

    public void RemoveLives(int livesToRemove)
    {
        if (!_isShieldActive)
            lives -= livesToRemove;
    }

    public int GetLives()
    {
        return lives;
    }

    public bool IsMoving()
    {
        return _isMoving;
    }

    public void SetMoving(bool moving)
    {
        _isMoving = moving;
    }

    public void OnSpeedChanged(float value)
    {
    }

}
