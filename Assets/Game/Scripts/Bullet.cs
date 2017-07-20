using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public string TargetTag;
    [HideInInspector] public int Damage;
    [SerializeField] float maxSpeed;
    [SerializeField] float minSpeed;
    [SerializeField] float destroyDelay;
    [SerializeField] GameObject Gfx;
    float speed;

    // Use this for initialization
    void Start()
    {
        var pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y, 0);
        Gfx.transform.position = pos;

        speed = Random.Range(minSpeed, maxSpeed);
        Destroy(gameObject, destroyDelay);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(TargetTag))
        {
            var c1 = collision.GetComponentInChildren<Entity>();
            var c2 = collision.GetComponentInParent<Entity>();
            var c = (c1 == null ? c2 : c1);
            if (c != null)
            {
                c.Hit(Damage);
            }
            else
            {
                Debug.LogError(collision.name + " should have a IHitteble in it's hierarchy!");
            }
            Destroy(gameObject);
        }
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += transform.right * speed * Time.fixedDeltaTime;
    }
}
