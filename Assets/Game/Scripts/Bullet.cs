using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    [SerializeField] float maxSpeed;
    [SerializeField] float minSpeed;
    float speed;

    // Use this for initialization
    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += Vector3.right * speed * Time.fixedDeltaTime;

    }
}
