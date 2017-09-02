using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Tower
{
    [Header("Range")]
    public float MaxAngle;
    public float MinAngle;

    [Header("Movement")]
    public float barrelSpeed;
    public float TowerSpeed;

    [Header("Tower")]
    public Transform TowerOrigin;

    [Header("Barrels")]
    public Transform BarrelOrigin;
    public Barrel[] Barrels;
}