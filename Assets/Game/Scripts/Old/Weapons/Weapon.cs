using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.Events;
using System.Threading.Tasks;
//using MovementEffects;

namespace SteelSkies
{

    public enum WeaponType { none = 0, Machinegun = 1, RocketLauncher = 2, Custom = 3 }

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
        public GameObject Graphics;

        public string Name;
        public WeaponType Type;
        public WeaponFireMod FireMod;
    }
    

    public class Weapon : MonoBehaviour
    {
        public WeaponData Data;
        [SerializeField] string enemyString;
        //List<Coroutine> routines = new List<Coroutine>();

        public bool Enabled { get; set; }
        
        Transform t;
        Vector3 point;
        float rot_z;

        Projectile pj;

        int x;
        int y;

        WeaponFireMod firemodeOnStart;
        void Start()
        {
            print("firemodeOnStart set!");
            firemodeOnStart = new WeaponFireMod()
            {
                SFX = Data.FireMod.SFX,
                Projectile = Data.FireMod.Projectile,
                Name = Data.FireMod.Name,
                FireSpots = Data.FireMod.FireSpots,
                FireRate = Data.FireMod.FireRate,
                Damage = Data.FireMod.Damage,
                BurstTimeSpace = Data.FireMod.BurstTimeSpace,
                Bursts = Data.FireMod.Bursts,
            };
        }

        public void Reset()
        {
            Data.FireMod = firemodeOnStart;
        }

        public void CreateProjectile()
        {
            for (x = 0; x < Data.FireMod.FireSpots.Length; x++)
            {
                t = Data.FireMod.FireSpots[x].Spot;
                
                /*
                if (Data.FireMod.FireSpots[x].FireParticle != null)
                    Data.FireMod.FireSpots[x].FireParticle.Emit(Data.FireMod.FireSpots[x].ParticleCount);
                */

                point = t.gameObject.transform.right;
                
                rot_z = Mathf.Atan2(point.y, point.x) * Mathf.Rad2Deg;

                GameObject go;
                go = AppServices.Instance.PoolManager.GetPooledPrefab(Data.FireMod.Projectile);
                
                go.transform.position = Data.FireMod.FireSpots[x].Spot.position;
                go.transform.rotation = Quaternion.Euler(0f, 0f, rot_z);

                go.transform.SetParent(GameServices.Instance.GameManager.ProjectilesField.transform);
                
                pj = go.GetComponent<Projectile>();
                pj.TargetTag = enemyString;
                pj.Damage = Data.FireMod.Damage;
                pj.SetUp();
            }
        }


        int shoot_i;
        async public void StartBurst() 
        {
            if (!Enabled)
                return;

            for (shoot_i = 0; shoot_i < Data.FireMod.Bursts; shoot_i++)
            {
                await TimeSpan.FromSeconds(Data.FireMod.BurstTimeSpace * shoot_i);
                Shoot();
                /*
                    GameServices.Instance.StartCoroutine(
                        CommonCoroutine.CallDelay(Shoot, Data.FireMod.BurstTimeSpace * shoot_i));
                */
            }
        }
        void Shoot()
        {
            if (Enabled)
            {
                CreateProjectile();
                PlaySFX();
            }
        }
        void PlaySFX()
        {
            AppServices.Instance.AudioManager.SoundEffectsManager.PlaySound(Data.FireMod.SFX);
        }
    }
}