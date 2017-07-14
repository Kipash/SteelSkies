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



public class JetController : MonoBehaviour
{
    [Header("GroundCheck")]
    [SerializeField] Transform groundCheckOrigin;
    [SerializeField] float groundCheckLength;
    [SerializeField] LayerMask groundCheckMask;

    [Header("Player")]
    [SerializeField] Directions playerSpeed;

    [Header("Tilt")]
    [SerializeField] float tiltSpeed;
    [SerializeField] float tiltSensitivity;
    [SerializeField] Directions tilt;

    [Header("Gun")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform[] shotSpawnSpots;

    Vector2 targetedTilt;

    bool isGrounded;

	void FixedUpdate()
    {
        isGrounded = Physics2D.Raycast(groundCheckOrigin.position, Vector2.down, groundCheckLength, groundCheckMask);
    }

    // Update is called once per frame
    void Update()
    {
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
            foreach(var p in shotSpawnSpots)
            {
                Instantiate(bullet, p.position, p.rotation);
            }
        }
        
        var rot = Quaternion.Lerp(
            transform.localRotation, 
            Quaternion.Euler(
                new Vector3(
                    targetedTilt.x * tiltSensitivity,
                    transform.localRotation.y,
                    targetedTilt.y * tiltSensitivity)),
            tiltSpeed * Time.deltaTime);

        transform.localRotation = new Quaternion(rot.x, transform.localRotation.y, rot.z, rot.w);
    }
}