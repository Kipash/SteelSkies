using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEngine.UI;
using MovementEffects;
using System.Collections.Generic;

[Serializable]
public class WeaponManager
{
    [Header("Gun")]
    [SerializeField] Transform WeaponHolder;
    [SerializeField] WeaponType primaryWeapon;

    Weapon[] allWeapons;
    public Weapon CurrentWeapon { get; private set; }

    bool disabled;
    public bool Disabled
    {
        get { return disabled; }
        set { disabled = value; IsShooting = !value; }
    }
    public bool IsShooting
    {
        get
        {
            return routines.Count != 0;
        }
        set
        {
            if (value && routines.Count == 0)
            {
                var hl = Timing.CallPeriodically(Mathf.Infinity, CurrentWeapon.Data.FireMod.FireRate, Shoot, Segment.Update);
                routines.Add(hl);
            }
            else if (!value && routines.Count != 0)
                Deactivate();
        }
    }

    List<CoroutineHandle> routines = new List<CoroutineHandle>();

    public void Deactivate()
    {
        CurrentWeapon.Deactivate();
        KillAllCoroutine();
        routines.Clear();
    }
    void KillAllCoroutine()
    {
        foreach (var x in routines)
            Timing.KillCoroutines(x);
    }

    public void Start()
    {
        allWeapons = WeaponHolder.GetComponentsInChildren<Weapon>();
        var wep = allWeapons.First(x => x.Data.Type == primaryWeapon);
        ChangeWeapon(wep);
    }
    
    void Shoot()
    {
        if (!Disabled)
            CurrentWeapon.Shoot();
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
}
