using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEngine.UI;
//using MovementEffects;
using System.Collections.Generic;

namespace SteelSkies
{
    [Serializable]
    public class PlayerWeaponry
    {
        [Header("Gun")]
        [SerializeField] Weapon machinegun;
        [SerializeField] Weapon rocketLauncher;
        [SerializeField] int damageDelta;
        [SerializeField] float firerateDelta;

        Weapon[] allWeapons;

        public void Start()
        {
            GameServices.Instance.LevelManager.OnLevelUp += LevelUp;
            allWeapons = new Weapon[] { machinegun, rocketLauncher };
            LevelUp(1);
            SetWeapons(true);

            foreach (var x in allWeapons)
            {
                x.Reset();
            }

            StartShoot();
        }

        void StartShoot()
        {
            for (int i = 0; i < allWeapons.Length; i++)
            {
                Shoot(allWeapons[i]);
            }
        }
        async void Shoot(Weapon wep)
        {
            if(wep.Enabled)
                wep.StartBurst();
            await TimeSpan.FromSeconds(wep.Data.FireMod.FireRate);
            Shoot(wep);
        }

        public void SetWeapons(bool state)
        {
            for (int i = 0; i < allWeapons.Length; i++)
            {
                allWeapons[i].Enabled = state;
            }
        }
        void LevelUp(int i)
        {
            float raw = (float)i / 5f;
            int upgrade = i - (int)raw * 5;
            switch (upgrade)
            {
                case 1:
                    machinegun.Data.FireMod.Damage += damageDelta;
                    break;  
                case 2:
                case 4:
                    machinegun.Data.FireMod.FireRate -= firerateDelta;
                    break;
                case 3:
                    machinegun.Data.FireMod.Bursts++;
                    break;
                case 5:
                    rocketLauncher.Data.FireMod.Bursts++;
                    break;
            }
        }
    }
}