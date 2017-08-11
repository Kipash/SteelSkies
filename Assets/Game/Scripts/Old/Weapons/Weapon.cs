using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Events;

public enum WeaponType { none = 0, Machinegun = 1, RocketLauncher = 2}

[Serializable]
public class FireSpot
{
    public Transform Spot;
    public ParticleSystem FireParticle;
    public float ParticleCount;
}

[Serializable]
public class WeaponFireMod
{
    public string Name;

    [Header("Visuals")]
    public GameObject Projectile;
    public FireSpot[] FireSpots;
    
    [Header("Stats")]
    public bool ShotPerBarrel;
    public float AmmoPerShot;
    public float PrewarmTime;
    public int Damage;
    public int AmmunitionPerShot;
    public int BurstTimeSpace;
    public float FireRate;
}

[Serializable]
public class WeaponData
{
    public bool HasAmmo
    {
        get
        {
            return Ammo != -1 && Ammo != 0;
        }
    }
    public int Ammo;
    public string Name;
    public WeaponType Type;
    public WeaponFireMod[] FireMods;
}

public class Weapon : MonoBehaviour
{
    public WeaponData Data;
    
    private void Start()
    {
        Data.FireMods = Data.FireMods.OrderBy(x => x.PrewarmTime).ToArray();
    }

    protected virtual void CreateProjectile(WeaponFireMod currFireMod)
    {
        var go = Instantiate(currFireMod.Projectile, currFireMod.FireSpots[0].Spot.position, currFireMod.FireSpots[0].Spot.rotation);
        var bu = go.GetComponent<Bullet>();
        bu.TargetTag = "Enemy";
        bu.Damage = currFireMod.Damage;
    }

    public void Shoot(float shootTime)
    {
        WeaponFireMod fMode = GetFireMod(shootTime);
        if (fMode != null)
            CreateProjectile(fMode);
    }

    public WeaponFireMod GetFireMod(float shootTime)
    {
        if (Data.FireMods.Length != 0)
        {
            for (int i = 0; i < Data.FireMods.Length; i++)
            {
                if (i + 1 != Data.FireMods.Length)
                {
                    if (Data.FireMods[i].PrewarmTime < shootTime && shootTime < Data.FireMods[i + 1].PrewarmTime)
                        return Data.FireMods[i];
                }
                else
                {
                    if (Data.FireMods[i].PrewarmTime < shootTime)
                        return Data.FireMods[i];
                }
            }
        }
        
        return null;
    }
}
