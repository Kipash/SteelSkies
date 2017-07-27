using UnityEngine;
using System.Collections;

public interface IHitteble
{
    int Health { get; }

    void Hit(int damage);
    void Die();
    void SetDefaultHP();
}
