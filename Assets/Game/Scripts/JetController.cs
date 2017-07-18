using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Directions
{
    public float Up;
    public float Down;
    public float Right;
    public float Left;
}

public class JetController : Entity
{
    [Header("GroundCheck")]
    [SerializeField] Transform groundCheckOrigin;
    [SerializeField] float groundCheckLength;
    [SerializeField] LayerMask groundCheckMask;

    [Header("Player")]
    [SerializeField] Directions playerSpeed;
    [SerializeField] int defaultHP;

    [Header("Tilt")]
    [SerializeField] float tiltSpeed;
    [SerializeField] float tiltSensitivity;
    [SerializeField] Directions tilt;
    [SerializeField] GameObject gfx;

    [Header("Gun")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform[] shotSpawnSpots;
    [SerializeField] float fireRate;
    [SerializeField] int damage;
    [SerializeField] ParticleSystem[] machinegunFX;

    Vector2 targetedTilt;

    bool isGrounded;

    float fireTrashold;

    private void Start()
    {
        SetDefaultHP(defaultHP);
    }

    RaycastHit2D[] hits;

    void FixedUpdate()
    {
        isGrounded = Physics2D.RaycastNonAlloc(groundCheckOrigin.position, Vector2.down, hits, groundCheckLength, groundCheckMask) > 0;

        gfx.transform.rotation = Quaternion.Euler
        (
            Mathf.LerpAngle
            (
                gfx.transform.rotation.eulerAngles.x,
                targetedTilt.x * tiltSensitivity,
                tiltSpeed * Time.fixedDeltaTime
            ),

            180,

            Mathf.LerpAngle
            (
                gfx.transform.rotation.eulerAngles.z,
                targetedTilt.y * tiltSensitivity,
                tiltSpeed * Time.fixedDeltaTime
            )
        );
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            if (Time.timeScale == 0)
                Time.timeScale = 1;
            else
                Time.timeScale = 0;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.localPosition += Vector3.left * playerSpeed.Left * Time.deltaTime;
            targetedTilt = new Vector2(targetedTilt.x, tilt.Left);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.localPosition += Vector3.right * playerSpeed.Right * Time.deltaTime;
            targetedTilt = new Vector2(targetedTilt.x, tilt.Right);
        }
        else
        {
            targetedTilt = new Vector2(targetedTilt.x, 0);
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.localPosition += Vector3.up * playerSpeed.Up * Time.deltaTime;
            targetedTilt = new Vector2(tilt.Up, targetedTilt.y);
        }
        else if (Input.GetKey(KeyCode.S) && !isGrounded)
        {
            transform.localPosition += Vector3.down * playerSpeed.Down * Time.deltaTime;
            targetedTilt = new Vector2(tilt.Down, targetedTilt.y);
        }
        else
        {
            targetedTilt = new Vector2(0, targetedTilt.y);
        }

        if(Input.GetKey(KeyCode.Space) && Time.time > fireTrashold)
        {
            fireTrashold = Time.time + fireRate;

            Shoot();
        }

        /*
        var rot = Quaternion.Lerp(
            gfx.transform.localRotation, 
            Quaternion.Euler(
                new Vector3(
                    targetedTilt.x * tiltSensitivity,
                    180,
                    targetedTilt.y * tiltSensitivity)),
            tiltSpeed * Time.deltaTime);

        gfx.transform.localRotation = new Quaternion(rot.x, transform.localRotation.y, rot.z, rot.w);
        */
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

        foreach (var f in machinegunFX)
            f.Emit(50);
        StartCoroutine(SpawnBullet());
    }
    IEnumerator SpawnBullet()
    {
        foreach (var p in shotSpawnSpots)
        {
            var go = Instantiate(bullet, p.position, p.rotation);
            var bu = go.GetComponent<Bullet>();
            bu.TargetTag = "Enemy";
            bu.Damage = damage;
            Destroy(go, 3);
            yield return null;
        }
    }
}