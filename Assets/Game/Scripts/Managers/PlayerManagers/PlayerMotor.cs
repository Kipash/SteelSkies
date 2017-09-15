using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PlayerMotor
{
    [Header("Raycasts")]
    [SerializeField] LayerMask rayCheckMask;
    [SerializeField] bool debugRays;

    [Header("BlockCheck")]
    [SerializeField] Transform blockCheckOrigin;
    [SerializeField] float blockCheckLength;
    Directions block = new Directions();

    [Header("Collision Check")]
    [SerializeField]
    Transform collisionCheckOrigin;
    [SerializeField] float collisionCheckLength;

    [Header("Movement")]
    [SerializeField] Directions playerSpeed;
    [SerializeField] float maxMotionMagnitude;

    [Header("Tilt")]
    [SerializeField] float tiltSpeed;
    [SerializeField] float tiltSensitivity;
    [SerializeField] Directions tilt;
    [SerializeField] GameObject gfx;
    Vector2 targetedTilt;

    [Header("Generic")]
    [SerializeField] Transform transform;

    public CollisionCallBack colCallback;
    RaycastHit2D[] hit;
    public bool Disabled;

    public void FixedUpdate()
    {
        CalculateTilt();
        CollisionChecks();
    }

    public void Update()
    {
        if(debugRays)
            DrawDebugLines();
    }
    void DrawDebugLines()
    {
        // - Down -
        Debug.DrawLine(blockCheckOrigin.position,
            new Vector3(blockCheckOrigin.position.x,
                blockCheckOrigin.position.y - blockCheckLength,
                blockCheckOrigin.position.z),
            Color.green);
        // - Up -
        Debug.DrawLine(blockCheckOrigin.position,
            new Vector3(blockCheckOrigin.position.x,
                blockCheckOrigin.position.y + blockCheckLength,
                blockCheckOrigin.position.z),
            Color.green);

        // - Right -
        Debug.DrawLine(collisionCheckOrigin.position,
            new Vector3(collisionCheckOrigin.position.x + collisionCheckLength,
                collisionCheckOrigin.position.y,
                collisionCheckOrigin.position.z),
            Color.red);

        // - Left -
        Debug.DrawLine(collisionCheckOrigin.position,
            new Vector3(collisionCheckOrigin.position.x - collisionCheckLength,
                collisionCheckOrigin.position.y,
                collisionCheckOrigin.position.z),
            Color.red);
    }
    void CollisionChecks()
    {
        block.Down = Physics2D.Raycast(blockCheckOrigin.position, Vector2.down, blockCheckLength, rayCheckMask) ? 1 : 0;
        block.Up = Physics2D.Raycast(blockCheckOrigin.position, Vector2.up, blockCheckLength, rayCheckMask) ? 1 : 0;

        hit = Physics2D.RaycastAll(collisionCheckOrigin.position, Vector2.right, collisionCheckLength, rayCheckMask);
        
        if (hit.Length > 0 && colCallback != null)
        {
            block.Right = 1;
            colCallback.Invoke(hit.First().transform.gameObject);
        }
        else
            block.Right = 0;


        hit = Physics2D.RaycastAll(collisionCheckOrigin.position, Vector2.left, collisionCheckLength, rayCheckMask);
        if (hit.Length > 0 && colCallback != null)
        {
            block.Left = 1;
            colCallback.Invoke(hit.First().transform.gameObject);
        }
        else
            block.Left = 0;
    }

    void CalculateTilt()
    {
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
        if (block.Up == 0)
        {
            Move(Vector3.up * playerSpeed.Up * Time.deltaTime, new Vector2(tilt.Up, targetedTilt.y));
        }
    }

    public void MoveDown()
    {
        if (block.Down == 0)
        {
            Move(Vector3.down * playerSpeed.Down * Time.deltaTime, new Vector2(tilt.Down, targetedTilt.y));
        }
    }

    public void MoveLeft()
    {
        if (block.Left == 0)
        {
            Move(Vector3.left * playerSpeed.Left * Time.deltaTime, new Vector2(targetedTilt.x, tilt.Left));
        }
    }

    public void MoveRight()
    {
        if (block.Right == 0)
        {
            Move(Vector3.right * playerSpeed.Right * Time.deltaTime, new Vector2(targetedTilt.x, tilt.Right));
        }
    }

    void Move(Vector3 motion, Vector2 tilt)
    {
        if(!Disabled)
        {
            var m = motion;
            if (motion.magnitude > maxMotionMagnitude)
                m = m.normalized * maxMotionMagnitude;
                    
            transform.localPosition += m;

            transform.localPosition = new Vector3(
                Mathf.Clamp(transform.localPosition.x,
                    -45.9f,
                    91.1f),
                Mathf.Clamp(transform.localPosition.y,
                    -5f,
                    64.50f),
                transform.localPosition.z);

            targetedTilt = tilt;

               
        }
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
