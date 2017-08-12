using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEngine.UI;

[Serializable]
public class WeaponManager
{
    [Header("Gun")]
    [SerializeField] Transform WeaponHolder;
    [SerializeField] WeaponType primaryWeapon;

    Weapon[] allWeapons;
    public float FireTrashold { get; private set; }

    Weapon primary;
    Weapon secundary;

    public Weapon CurrentWeapon { get; private set; }
    
    public float GetCurrentWarmUp
    {
        get
        {
            if (FireTrashold == -1)
                return 0;
            else
                return Mathf.Clamp01((Time.time - FireTrashold) / CurrentWeapon.Data.FireMods[1].PrewarmTime);
        }
    }

    public void Start()
    {
        allWeapons = WeaponHolder.GetComponentsInChildren<Weapon>();
        primary = allWeapons.First(x => x.Data.Type == primaryWeapon);
        ChangeWeapon(primary);
        FireTrashold = -1;
    }

    public void PrewarmShot()
    {
        FireTrashold = Time.time;
    }
    public void Shoot()
    {
        CurrentWeapon.Shoot(Time.time - FireTrashold);
        FireTrashold = -1;
        if (!CurrentWeapon.Data.HasAmmo)
        {
            if (CurrentWeapon == primary)
                Debug.LogErrorFormat("primary({0}) weapon has no ammo({1})! It should be set to '-1' - infinite", CurrentWeapon.Data.Name, CurrentWeapon.Data.Ammo);

            ChangeWeapon(primary);
        }
    }
    void ChangeWeapon(Weapon wep)
    {
        CurrentWeapon = wep;

        allWeapons
            .Except(new Weapon[] { wep })
            .Select((x) =>
            {
                x.Data.Graphics.SetActive(false);
                return x;
            });

        wep.Data.Graphics.SetActive(true);
    }

    public void SetSecundary(Weapon wep)
    {
        secundary = wep;
        ChangeWeapon(secundary);
    }
}
