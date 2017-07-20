using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
    bool isGrounded;

    [Header("Player")]
    [SerializeField] Directions playerSpeed;
    [SerializeField] int defaultHP;

    [Header("Tilt")]
    [SerializeField] float tiltSpeed;
    [SerializeField] float tiltSensitivity;
    [SerializeField] Directions tilt;
    [SerializeField] GameObject gfx;
    Vector2 targetedTilt;

    [Header("Gun")]
    [SerializeField] Transform WeaponHolder;
    Weapon[] weapons;
    float fireTrashold;

    ///*

    [Header("Temp")]
    [SerializeField] Text text;
    [SerializeField] Slider slider;
    [SerializeField] Transform[] mods;
    [SerializeField] float speed;

    bool trackTime;

    //*/
    

    private void Start()
    {
        SetDefaultHP(defaultHP);

        weapons = WeaponHolder.GetComponentsInChildren<Weapon>();
    }

    RaycastHit2D[] hits;

    void FixedUpdate()
    {
        //int contacts = Physics2D.RaycastNonAlloc(groundCheckOrigin.position, Vector2.down, hits, groundCheckLength, groundCheckMask);
        //print(contacts);
        //isGrounded = contacts > 0;

        isGrounded = Physics2D.Raycast(groundCheckOrigin.position, Vector2.down, groundCheckLength, groundCheckMask);

        Debug.DrawLine(groundCheckOrigin.position, new Vector3(groundCheckOrigin.position.x, groundCheckOrigin.position.y - groundCheckLength, groundCheckOrigin.position.z), Color.green);

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
        if (trackTime)
        {
            var rawTime = Time.time - fireTrashold;
            var fMod = weapons[0].GetFireMod(rawTime);
            var time = (((int)((Time.time - fireTrashold) * 100f)) / 100f).ToString();
            text.text = (fMod != null ? fMod.Name : "") + "\n" + time;
            slider.value = rawTime;


            int index = 0;
            for (int i = 0; i < weapons[0].Data.FireMods.Length; i++)
            {
                if (weapons[0].Data.FireMods[i] == fMod)
                {
                    index = i;
                    break;
                }
            }

            for (int i = 0; i < mods.Length; i++)
            {
                var dif = 1 - Mathf.Abs(i - index) * 0.2f;
                mods[i].localScale = Vector3.Lerp(mods[i].localScale, new Vector3(dif, dif, dif), speed * Time.deltaTime);
            }
        }
        else
        {
            text.text = "";
            for (int i = 0; i < mods.Length; i++)
            {
                var dif = 1 - Mathf.Abs(i - 0) * 0.2f;
                mods[i].localScale = Vector3.Lerp(mods[i].localScale, new Vector3(dif, dif, dif), speed * Time.deltaTime);
            }
        }


        if (Input.GetKeyDown(KeyCode.F1))
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

        if(Input.GetKeyDown(KeyCode.Space))
        {
            fireTrashold = Time.time;
            trackTime = true;
            slider.maxValue = weapons[0].Data.FireMods.Last().PrewarmTime;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            weapons[0].Shoot(Time.time - fireTrashold);
            trackTime = false;

            slider.value = 0;
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
}