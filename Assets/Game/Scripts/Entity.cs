using UnityEngine;
using System.Collections;
using System;

public abstract class Entity : MonoBehaviour
{
    public int Health { get; private set; }

    public virtual void Hit(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            Die();
        }
    }
    public virtual void Die()
    {
        Destroy(gameObject);
    }
    protected void SetDefaultHP(int hp)
    {
        Health = hp;
    }
}
