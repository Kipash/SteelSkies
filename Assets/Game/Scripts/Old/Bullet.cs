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
    private void OnEnable()
    {
        StartCoroutine(OnEnabled());
    }

    IEnumerator OnEnabled()
    {
        yield return null;

        var pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y, 0);
        Gfx.transform.position = pos;

        speed = Random.Range(minSpeed, maxSpeed);
        Invoke("Destroy", destroyDelay);
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
            Destroy();
        }
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += transform.right * speed * Time.fixedDeltaTime;
    }

    //???
    float Snap(float val)
    {
        int neg = val < 0 ? -1 : 1;

        var wVal = Mathf.Abs(val);
        var res = wVal < 45 ? 0 : wVal;
        return res * neg;
    }

    void Destroy()
    {
        if (!Services.Instance.PoolManager.DeactivatePrefab(gameObject))
        {
            Debug.LogErrorFormat("GameObject {0} is acting as PooledPrefab, object was destroyed!", gameObject.name);
            Destroy(gameObject);
        }
    }
}
