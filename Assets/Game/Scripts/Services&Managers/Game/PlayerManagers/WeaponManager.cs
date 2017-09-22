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

        Coroutine currCoroutine;

        Weapon wep;

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
                return currCoroutine != null;
            }
            set
            {
                if (value && currCoroutine == null)
                {
                    currCoroutine = GameServices.Instance.StartCoroutine(CommonCoroutine.CallRepeatedly(Shoot, CurrentWeapon.Data.FireMod.FireRate));
                }
                else if (!value && currCoroutine != null)
                    Deactivate();
            }
        }

        public void Deactivate()
        {
            CurrentWeapon.Deactivate();
            GameServices.Instance.StopCoroutine(currCoroutine);
            currCoroutine = null;
        }

        public void Start()
        {
            allWeapons = WeaponHolder.GetComponentsInChildren<Weapon>();
            wep = allWeapons.First(x => x.Data.Type == primaryWeapon);
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
}