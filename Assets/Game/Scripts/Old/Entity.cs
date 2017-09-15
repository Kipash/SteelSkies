using UnityEngine;
using System.Collections;
using System;
using MovementEffects;

public abstract class Entity : MonoBehaviour
{
    public int Health { get; private set; }
    public bool Invisible { get; set; }
    float crashDelay = 0.2f;

    public virtual void Hit(int damage)
    {
        if (damage == -1)
            Health = 0;
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
    public void Crash(int damage)
    {
        if (Invisible)
            return;

        Hit(damage);
        Invisible = true;
        Timing.Instance.CallDelayedOnInstance(crashDelay, () => { Invisible = false; }, GetType().ToString());

    }
}
