using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class Explosion
{
    public float Radius;
    public float Duration;
    public int Damage;
    public PooledPrefabs Prefab;
    public LayerMask Mask;
}
