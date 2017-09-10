using UnityEngine;
using System;
using System.Collections;
using MovementEffects;
using System.Collections.Generic;

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

    List<CoroutineHandle> routines = new List<CoroutineHandle>();

    public void Deactivate()
    {
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
        if (disable)
            return;

        Timing.Instance.AddTag(EnemyTimingTag, false);

        routines.Add(Timing.CallPeriodically(Mathf.Infinity,
             wep.Data.FireMod.FireRate + UnityEngine.Random.Range(0, fireRateDeviation),
             Shoot, EnemyTimingTag));
    }

    public void RotateTowers()
    {
        if (disable)
            return;

        foreach (var t in Towers)
        {
            if (t.TowerOrigin.gameObject.active && !disable && Target != null)
            {
                var towerDir = Target.position - t.TowerOrigin.position;
                var towerRot = Quaternion.LookRotation(new Vector3(towerDir.x, 0, towerDir.z));
                t.TowerOrigin.rotation = Quaternion.Lerp(t.TowerOrigin.rotation, towerRot, t.TowerSpeed * Time.deltaTime);

                var barrelDir = Target.position - t.BarrelOrigin.position;
                var barrelRot = Quaternion.LookRotation(new Vector3(barrelDir.x, barrelDir.y, 0));
                barrelRot.y = 0;
                barrelRot.z = 0;
                t.BarrelOrigin.localRotation = Quaternion.LerpUnclamped(t.BarrelOrigin.localRotation, barrelRot, t.barrelSpeed * Time.deltaTime);

                //t.BarrelOrigin.rotation = Quaternion.Lerp(t.BarrelOrigin.rotation, barrelRot, t.barrelSpeed * Time.deltaTime);

                var rot = t.BarrelOrigin.localRotation;

                rot.x = Mathf.Clamp(rot.x, t.MinAngle / 180, t.MaxAngle / 180);

                t.BarrelOrigin.localRotation = rot;

                if(debug)
                    Debug.DrawLine(Target.position, t.BarrelOrigin.position, Color.cyan);
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
            if (t.TowerOrigin.gameObject.active)
            {
                wep.Shoot();
            }
        }
    }
}
