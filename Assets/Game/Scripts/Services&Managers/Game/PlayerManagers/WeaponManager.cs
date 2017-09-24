using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEngine.UI;
//using MovementEffects;
using System.Collections.Generic;

namespace Aponi
{
    [Serializable]
    public class WeaponManager
    {
        [Header("Gun")]
        [SerializeField]
        Transform WeaponHolder;
        [SerializeField] WeaponType primaryWeapon;

        Weapon[] allWeapons;
        public Weapon CurrentWeapon { get; private set; }

        Weapon wep;

        public bool Disabled;

        public void Start()
        {
            allWeapons = WeaponHolder.GetComponentsInChildren<Weapon>();
            wep = allWeapons.First(x => x.Data.Type == primaryWeapon);
            ChangeWeapon(wep);
            CurrentWeapon.Enabled = true;

            StartShoot();
        }

        async void StartShoot()
        {
            while (true)
            {
                if (!Disabled)
                {
                    CurrentWeapon.StartBurst();
                    await TimeSpan.FromSeconds(CurrentWeapon.Data.FireMod.FireRate);
                }
                else
                {
                    await TimeSpan.FromSeconds(Time.deltaTime);
                }
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
    }
}