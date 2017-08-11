using UnityEngine;
using System.Collections;
using System;
using System.Linq;

[Serializable]
public class WeaponManager
{
    [Header("Gun")]
    [SerializeField] Transform WeaponHolder;
    [SerializeField] WeaponType primaryWeapon;
    Weapon[] allWeapons;
    float fireTrashold;

    Weapon primary;
    Weapon secundary;

    public Weapon CurrentWeapon { get; private set; }
    
    public void Start()
    {
        allWeapons = WeaponHolder.GetComponentsInChildren<Weapon>();
        primary = allWeapons.First(x => x.Data.Type == primaryWeapon);
        ChangeWeapon(primary);
    }

    public void PrewarmShot()
    {
        fireTrashold = Time.time;
    }
    public void Shoot()
    {
        CurrentWeapon.Shoot(Time.time - fireTrashold);

        if(!CurrentWeapon.Data.HasAmmo)
        {
            if (CurrentWeapon == primary)
                Debug.LogError("primary weapon has not ammo! It should be set to '-1' - infinite");

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
                x.gameObject.SetActive(false);
                return x;
            });

        wep.gameObject.SetActive(true);
    }

    public void SetSecundary(Weapon wep)
    {
        secundary = wep;
        ChangeWeapon(secundary);
    }
}
