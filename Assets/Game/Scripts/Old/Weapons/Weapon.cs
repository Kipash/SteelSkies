using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Events;
//using MovementEffects;

namespace Aponi
{

    public enum WeaponType { none = 0, Machinegun = 1, RocketLauncher = 2 }

    [Serializable]
    public class FireSpot
    {
        public Transform Spot;
        public ParticleSystem FireParticle;
        public int ParticleCount;
    }

    [Serializable]
    public class WeaponFireMod
    {
        public string Name;

        [Header("Visuals")]
        public PooledPrefabs Projectile;
        public SoundEffects SFX;
        public FireSpot[] FireSpots;

        [Header("Stats")]
        public int Damage;
        public float BurstTimeSpace;
        public float Bursts;
        public float FireRate;
    }

    [Serializable]
    public class WeaponData
    {
        public bool HasAmmo
        {
            get
            {
                //return (Ammo != 0 ? (Ammo == -1 ? true : false) : (Ammo > 0));
                return Ammo == -1 || Ammo > 0;
            }
        }
        public int Ammo;

        public GameObject Graphics;

        public string Name;
        public WeaponType Type;
        public WeaponFireMod FireMod;
    }
    

    public class Weapon : MonoBehaviour
    {
        public WeaponData Data;
        [SerializeField] string enemyString;
        List<Coroutine> routines = new List<Coroutine>();

        GameObject go;
        Transform t;
        Vector3 point;
        float rot_z;

        Projectile pj;

        public void CreateProjectile()
        {
            foreach (var fireSpot in Data.FireMod.FireSpots)
            {
                t = fireSpot.Spot;

                if (fireSpot.FireParticle != null)
                    fireSpot.FireParticle.Emit(fireSpot.ParticleCount);

                point = t.gameObject.transform.right;
                rot_z = Mathf.Atan2(point.y, point.x) * Mathf.Rad2Deg;

                go = AppServices.Instance.PoolManager.GetPooledPrefab(Data.FireMod.Projectile);
                go.transform.position = fireSpot.Spot.position;
                go.transform.rotation = Quaternion.Euler(0f, 0f, rot_z);

                go.transform.SetParent(GameServices.Instance.GameManager.ProjectilesField.transform);

                foreach (var x in go.GetComponentsInChildren<TrailRenderer>())
                    x.Clear();

                pj = go.GetComponent<Projectile>();
                pj.TargetTag = enemyString;
                pj.Damage = Data.FireMod.Damage;
            }
        }

        int shoot_i;
        public void Shoot()
        {
            for (shoot_i = 0; shoot_i < Data.FireMod.Bursts; shoot_i++)
            {
                routines.Add(
                    GameServices.Instance.StartCoroutine(
                        CommonCoroutine.CallDelay(
                            () => { CreateProjectile(); PlaySFX(); },
                            Data.FireMod.BurstTimeSpace * shoot_i)));
            }
        }
        public void Deactivate()
        {
            KillAllCoroutine();
            routines.Clear();
        }
        void KillAllCoroutine()
        {
            foreach (var x in routines)
                GameServices.Instance.StopCoroutine(x);
        }
        void PlaySFX()
        {
            AppServices.Instance.AudioManager.SoundEffectsManager.PlaySound(Data.FireMod.SFX);
        }
    }
}