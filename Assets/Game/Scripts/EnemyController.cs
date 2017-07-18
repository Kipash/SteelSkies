using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class Tower
{
    [Header("Range")]
    public float MaxAngle;
    public float MinAngle;

    [Header("Movement")]
    public float CannonSpeed;
    public float TowerSpeed;

    [Header("Tower")]
    public Transform TowerOrigin;

    [Header("Barrels")]
    public Barrel[] Barrels;

}

[Serializable]
public class Barrel
{
    [Header("SpawnSpots")]
    public Transform SpawnSpot;

    [Header("Barrel")]
    public Transform BarrelOrigin;

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

    [Header("Movement")]
    [SerializeField] float speed;

    [Header("Weaponry")]
    [SerializeField] float fireRate;
    [SerializeField] int damage;
    [SerializeField] GameObject bullet;
    [SerializeField] Tower[] Towers;
    
    void Start()
    {
        //shootTarget = GameObject.FindGameObjectWithTag(targetTag).transform;
        StartCoroutine(Patrol());
        SetDefaultHP(defaultHP);
        InvokeRepeating("Shoot", fireRate + UnityEngine.Random.Range(0.1f, 1), fireRate);
    }

    void Update()
    {
        transform.localPosition = Vector2.Lerp(transform.localPosition, patrolTarget, Time.deltaTime * speed);
        foreach(var t in Towers)
        {
            var rot = Quaternion.LookRotation(shootTarget.position - t.TowerOrigin.position);
            //t.TowerOrigin.rotation = new Quaternion(0, rot.y, 0, 1);
            foreach(var b in t.Barrels)
                b.BarrelOrigin.localRotation = new Quaternion(rot.x + 0.25f, 0, 0, 1);
        }
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
        StartCoroutine(Hide());
    }

    IEnumerator Hide()
    {

        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        yield return new WaitForSeconds(2);

        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = true;

        SetDefaultHP(defaultHP);
    }

    void Shoot()
    {
        foreach (var t in Towers)
        {
            if (t.TowerOrigin.gameObject.active)
            {
                foreach (var b in t.Barrels)
                {
                    foreach (var e in b.Effects)
                        e.Particle.Emit(e.Amount);

                    var p = b.SpawnSpot;
                    var go = Instantiate(bullet, p.position, p.rotation);
                    var bu = go.GetComponent<Bullet>();
                    bu.TargetTag = "Player";
                    bu.Damage = damage;
                    Destroy(go, 3);
                }
            }
        }
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
}
