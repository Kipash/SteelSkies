using UnityEngine;
using System;
using System.Collections;
//using MovementEffects;
using System.Collections.Generic;

namespace Aponi
{

    [Serializable]
    public class EnemyWeaponry
    {
        //[SerializeField] Transform transform;

        [SerializeField] bool disable;
        [SerializeField] bool debug;
        [SerializeField] Weapon wep;
        [SerializeField] float fireRateDeviation;


        [SerializeField] Tower[] Towers;

        [HideInInspector] public Transform Target;

        public const string EnemyTimingTag = "EnemyTag";

        Coroutine currCoroutine;

        public void Deactivate()
        {
            if(GameServices.Initialize && currCoroutine != null)
                GameServices.Instance.StopCoroutine(currCoroutine);

            currCoroutine = null;
        }

        public void Start()
        {
            if (disable)
                return;

            //Timing.Instance.AddTag(EnemyTimingTag, false);
            if (GameServices.Initialize)
            {
                currCoroutine = GameServices.Instance.StartCoroutine(
                    CommonCoroutine.CallRepeatedly(
                        Shoot,
                        wep.Data.FireMod.FireRate + UnityEngine.Random.Range(0, fireRateDeviation)));
            }
        }

        Vector3 v3;
        Quaternion q;
        public void RotateTowers()
        {
            if (disable)
                return;

            foreach (var t in Towers)
            {
                if (t.TowerOrigin.gameObject.activeInHierarchy && !disable && Target != null)
                {
                    v3 = Target.position - t.TowerOrigin.position;
                    this.q = Quaternion.LookRotation(new Vector3(v3.x, 0, v3.z));
                    t.TowerOrigin.rotation = Quaternion.Lerp(t.TowerOrigin.rotation, this.q, t.TowerSpeed * Time.deltaTime);

                    v3 = Target.position - t.BarrelOrigin.position;
                    this.q = Quaternion.LookRotation(new Vector3(v3.x, v3.y, 0));
                    this.q.y = 0;
                    this.q.z = 0;
                    t.BarrelOrigin.localRotation = Quaternion.LerpUnclamped(t.BarrelOrigin.localRotation, this.q, t.barrelSpeed * Time.deltaTime);

                    //t.BarrelOrigin.rotation = Quaternion.Lerp(t.BarrelOrigin.rotation, barrelRot, t.barrelSpeed * Time.deltaTime);

                    q = t.BarrelOrigin.localRotation;

                    q.x = Mathf.Clamp(q.x, t.MinAngle / 180, t.MaxAngle / 180);

                    t.BarrelOrigin.localRotation = q;

                    if (debug)
                    {
                        UnityEngine.Debug.DrawLine(Target.position, t.BarrelOrigin.position, Color.cyan);
                        UnityEngine.Debug.DrawLine(t.BarrelOrigin.position, t.BarrelOrigin.position + (t.BarrelOrigin.forward * 1000), Color.white);
                    }
                }
            }
        }
        void Shoot()
        {
            if (disable)
                return;

            AppServices.Instance.AudioManager.SoundEffectsManager.PlaySound(wep.Data.FireMod.SFX);

            foreach (var t in Towers)
            {
                if (t.TowerOrigin.gameObject.activeInHierarchy)
                {
                    wep.Shoot();
                }
            }
        }
    }
}