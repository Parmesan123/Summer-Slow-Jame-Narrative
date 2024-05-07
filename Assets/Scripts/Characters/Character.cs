using System;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] protected int health;
    public event Action OnDeathEvent;

    public virtual void TakeDamage(int damage, Vector2 damageDirection)
    {
        health -= damage;
        if (health <= 0)
            Death();
    }

    protected virtual void Death()
    {
        OnDeathEvent?.Invoke();
    }
}
