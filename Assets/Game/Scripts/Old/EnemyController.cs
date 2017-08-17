using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;

[Serializable]
public class Tower
{
    [Header("Range")]
    public float MaxAngle;
    public float MinAngle;

    [Header("Movement")]
    public float barrelSpeed;
    public float TowerSpeed;

    [Header("Tower")]
    public Transform TowerOrigin;

    [Header("Barrels")]
    public Transform BarrelOrigin;
    public Barrel[] Barrels;
}

[Serializable]
public class Barrel
{
    [Header("SpawnSpots")]
    public Transform SpawnSpot;

    [Header("Effects")]
    public ParticleEffect[] Effects;
}
[SerializeField]
public class Barrels
{
    public Barrel[] InnerBarrels;
}


[Serializable]
public class ParticleEffect
{
    public ParticleSystem Particle;
    public int Amount;

}

public class EnemyController : Entity
{
    [Header("AI")]
    [SerializeField] Vector2[] points;
    [SerializeField] float waypointMinDistance;
    [SerializeField] int defaultHP;
    [SerializeField] string targetTag;
    Vector2 patrolTarget;
    [SerializeField] Transform shootTarget;
    [SerializeField] float explosionDuration;
    [SerializeField] Renderer renderer;

    [Header("Movement")]
    [SerializeField] float speed;

    [Header("Weaponry")]
    [SerializeField] bool disable;
    [SerializeField] bool optimize = true;
    [SerializeField] float fireRate;
    [SerializeField] int damage;
    [SerializeField] GameObject bullet;
    [SerializeField] Tower[] Towers;

    [Header("Explosion")]
    [SerializeField] Explosion explosion;

    DelayedCall explosionCallBack = new DelayedCall();

    private void OnEnable()
    {
        renderer.materials[0].SetColor("_EmissionColor", Color.black);
        //shootTarget = GameObject.FindGameObjectWithTag(targetTag).transform;
        StartCoroutine(Patrol());
        SetDefaultHP(defaultHP);
        InvokeRepeating("Shoot", fireRate + UnityEngine.Random.Range(0.1f, 5), fireRate);
        if (shootTarget == null)
        {
            var go = GameObject.FindGameObjectWithTag(targetTag);
            shootTarget = go != null ? go.transform : null;
        }
    }
    private void OnDisable()
    {
        CancelInvoke("Shoot");
    }

    void Update()
    {
        transform.localPosition = Vector2.Lerp(transform.localPosition, patrolTarget, Time.deltaTime * speed);
        foreach(var t in Towers)
        {
            if (t.TowerOrigin.gameObject.active)
            {
                var towerDir = shootTarget.position - t.TowerOrigin.position;
                var towerRot = Quaternion.LookRotation(new Vector3(towerDir.x, 0, towerDir.z));
                t.TowerOrigin.rotation = Quaternion.Lerp(t.TowerOrigin.rotation, towerRot, t.TowerSpeed * Time.deltaTime);

                var barrelDir = shootTarget.position - t.BarrelOrigin.position;
                var barrelRot = Quaternion.LookRotation(new Vector3(barrelDir.x, barrelDir.y, 0));
                barrelRot.y = 0;
                barrelRot.z = 0;
                t.BarrelOrigin.localRotation = Quaternion.LerpUnclamped(t.BarrelOrigin.localRotation, barrelRot, t.barrelSpeed * Time.deltaTime);

                //t.BarrelOrigin.rotation = Quaternion.Lerp(t.BarrelOrigin.rotation, barrelRot, t.barrelSpeed * Time.deltaTime);

                var rot = t.BarrelOrigin.localRotation;

                rot.x = Mathf.Clamp(rot.x, t.MinAngle / 180, t.MaxAngle / 180);

                t.BarrelOrigin.localRotation = rot;

                Debug.DrawLine(shootTarget.position, t.BarrelOrigin.position, Color.cyan);
            }
        }
    }

    public override void Hit(int damage)
    {
        StartCoroutine(HitEffect());
        base.Hit(damage);
    }
    IEnumerator HitEffect()
    {
        renderer.materials[0].SetColor("_EmissionColor", Color.red);
        yield return new WaitForSeconds(0.1f);
        renderer.materials[0].SetColor("_EmissionColor", Color.black);
    }

    bool loop;
    IEnumerator Patrol()
    {
        while (true)
        {
            patrolTarget = points[UnityEngine.Random.Range(0, points.Length - 1)];
            loop = true;
            while (loop)
            {
                if (Vector2.Distance(patrolTarget, transform.localPosition) < waypointMinDistance)
                    loop = false;
                yield return null;
            }
        }
    }

    public override void Die()
    {
        AppServices.Instance.AudioManager.SoundEffectsManager.PlaySound(SoundEffects.Explosion);
        var ex = AppServices.Instance.PoolManager.GetPooledPrefab(PooledPrefabs.Explsion);
        ex.transform.position = transform.position;

        explosionCallBack.Delay = explosionDuration;
        explosionCallBack.CallBack = () => { AppServices.Instance.PoolManager.DeactivatePrefab(ex); };
        AppServices.Instance.StaticCoroutines.Invoke(explosionCallBack);

        
        GameServices.Instance.ChallengeManager.DeactivateEntity(gameObject);
        SetDefaultHP(defaultHP);
    }

    void Shoot()
    {
        if (disable)
            return;

        AppServices.Instance.AudioManager.SoundEffectsManager.PlaySound(SoundEffects.Machinegun);

        foreach (var t in Towers)
        {
            if (t.TowerOrigin.gameObject.active)
            {
                foreach (var b in t.Barrels)
                {
                    foreach (var e in b.Effects)
                        e.Particle.Emit(e.Amount);

                    var p = b.SpawnSpot;

                    GameObject go;
                    if (optimize)
                    {
                        go = AppServices.Instance.PoolManager.GetPooledPrefab(PooledPrefabs.Bullet);
                        go.transform.position = p.position;
                        go.transform.rotation = p.rotation;
                        foreach (var x in go.GetComponentsInChildren<TrailRenderer>())
                            x.Clear();
                    }
                    else
                    {
                        go = Instantiate(bullet, p.position, p.rotation);
                    }
                    var bu = go.GetComponent<Projectile>();
                    bu.TargetTag = "Player";
                    bu.Damage = damage;

                }
            }
        }
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
}
