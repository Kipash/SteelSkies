using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Barrel
{
    [Header("SpawnSpots")]
    public Transform SpawnSpot;

    [Header("Effects")]
    public ParticleEffect[] Effects;
}