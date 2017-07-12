using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Tilt
{
    public float Up;
    public float Down;
    public float Right;
    public float Left;
}

public class JetController : MonoBehaviour
{
    [SerializeField] float playerSpeed;
    [SerializeField] float tiltSpeed;
    [SerializeField] float tiltSensitivity;
    [SerializeField] Tilt tilt;

    Vector2 targetedTilt;

	void Start ()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.localPosition += Vector3.left * playerSpeed * Time.deltaTime;
            targetedTilt = new Vector2(targetedTilt.x, tilt.Left);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.localPosition += Vector3.right * playerSpeed * Time.deltaTime;
            targetedTilt = new Vector2(targetedTilt.x, tilt.Right);
        }
        else
        {
            targetedTilt = new Vector2(targetedTilt.x, 0);
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.localPosition += Vector3.up * playerSpeed * Time.deltaTime;
            targetedTilt = new Vector2(tilt.Up, targetedTilt.y);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.localPosition += Vector3.down * playerSpeed * Time.deltaTime;
            targetedTilt = new Vector2(tilt.Down, targetedTilt.y);
        }
        else
        {
            targetedTilt = new Vector2(0, targetedTilt.y);
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
