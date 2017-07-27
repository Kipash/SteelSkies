using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PlayerMotor
{
    [Header("GroundCheck")]
    [SerializeField] Transform groundCheckOrigin;
    [SerializeField] float groundCheckLength;
    [SerializeField] LayerMask groundCheckMask;
    bool isGrounded;

    [Header("Movement")]
    [SerializeField] Directions playerSpeed;

    [Header("Tilt")]
    [SerializeField] float tiltSpeed;
    [SerializeField] float tiltSensitivity;
    [SerializeField] Directions tilt;
    [SerializeField] GameObject gfx;
    Vector2 targetedTilt;

    [Header("Generic")]
    Transform transform;

    public void FixedUpdate()
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

    public void MoveUp()
    {
        transform.localPosition += Vector3.up * playerSpeed.Up * Time.deltaTime;
        targetedTilt = new Vector2(tilt.Up, targetedTilt.y);        
    }

    public void MoveDown()
    {
        if (!isGrounded)
        {
            transform.localPosition += Vector3.down * playerSpeed.Down * Time.deltaTime;
            targetedTilt = new Vector2(tilt.Down, targetedTilt.y);
        }
    }

    public void MoveLeft()
    {
        transform.localPosition += Vector3.left * playerSpeed.Left * Time.deltaTime;
        targetedTilt = new Vector2(targetedTilt.x, tilt.Left);
    }

    public void MoveRight()
    {
        transform.localPosition += Vector3.right * playerSpeed.Right * Time.deltaTime;
        targetedTilt = new Vector2(targetedTilt.x, tilt.Right);
    }

    void ResetHorizontal()
    {
        targetedTilt = new Vector2(targetedTilt.x, 0);
    }
    void ResetVertical()
    {
        targetedTilt = new Vector2(0, targetedTilt.y);
    }

    public void PlayerMotor_OnCallbacksDone(RegisteredKeys[] keys)
    {
        if (!keys.Contains(RegisteredKeys.MoveLeft) && !keys.Contains(RegisteredKeys.MoveRight))
            ResetHorizontal();
        if (!keys.Contains(RegisteredKeys.MoveUp) && !keys.Contains(RegisteredKeys.MoveDown))
            ResetVertical();
    }
}
