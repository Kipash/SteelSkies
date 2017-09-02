using UnityEngine;
using System;
using System.Collections;
using MovementEffects;

[Serializable]
public class EnemyWeaponry
{
    //[SerializeField] Transform transform;

    [SerializeField] bool disable;
    [SerializeField] bool debug;
    [SerializeField] float fireRate;
    [SerializeField] float fireRateDeviation;
    [SerializeField] int damage;
    [SerializeField] PooledPrefabs projectile;
    [SerializeField] SoundEffects shootSFX;
    [SerializeField] Tower[] Towers;

    [HideInInspector] public Transform Target;

    public const string EnemyTimingTag = "EnemyTag";

    public void Start()
    {
        if (disable)
            return;

        Timing.Instance.AddTag(EnemyTimingTag, false);
        Timing.CallPeriodically(Mathf.Infinity, fireRate + UnityEngine.Random.Range(0, fireRateDeviation), Shoot, EnemyTimingTag);
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

        AppServices.Instance.AudioManager.SoundEffectsManager.PlaySound(shootSFX);

        foreach (var t in Towers)
        {
            if (t.TowerOrigin.gameObject.active)
            {
                foreach (var b in t.Barrels)
                {
                    foreach (var e in b.Effects)
                        e.Particle.Emit(e.Amount);

                    var p = b.SpawnSpot;

                    var go = AppServices.Instance.PoolManager.GetPooledPrefab(projectile);
                    go.transform.position = p.position;
                    go.transform.rotation = p.rotation;
                    foreach (var x in go.GetComponentsInChildren<TrailRenderer>())
                        x.Clear();

                    var bu = go.GetComponent<Projectile>();
                    if (bu != null && Target != null)
                    {
                        //
                        var gObject = Target.gameObject;
                        var tag = gObject.tag;
                        bu.TargetTag = tag;
                        bu.Damage = damage;
                    }
                }
            }
        }
    }
}
