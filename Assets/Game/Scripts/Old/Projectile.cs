using UnityEngine;
using System.Collections;

namespace Aponi
{
    public class Projectile : MonoBehaviour
    {
        [HideInInspector] public string TargetTag;
        [HideInInspector] public int Damage;

        [SerializeField] bool explodeOnImpact;

        [SerializeField] float maxSpeed;
        [SerializeField] float minSpeed;
        [SerializeField] float destroyDelay;
        [SerializeField] GameObject Gfx;

        [SerializeField] Explosion explosion;

        float speed;
        RaycastHit2D[] hits;

        Vector3 pos;

        GameObject pooledG;
        float dis;
        float diff;
        float dmg;

        Entity e1;
        Entity e2;
        Entity e;

        bool initialized;
        TrailRenderer[] trails;

        int i;

        private void OnEnable()
        {
            if (!initialized)
                Initialize();

            pos = transform.position;
            transform.position = new Vector3(pos.x, pos.y, 0);
            Gfx.transform.position = pos;

            speed = maxSpeed;
            Invoke("Destroy", destroyDelay);
        }

        void Initialize()
        {
            trails = GetComponentsInChildren<TrailRenderer>();
            initialized = true;
        }

        public void SetUp()
        {
            for (i = 0; i < trails.Length; i++)
            {
                trails[i].Clear();
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!gameObject.activeInHierarchy)
                return;

            if (collision.CompareTag(TargetTag) && collision.gameObject.activeInHierarchy)
            {
                e1 = collision.GetComponentInChildren<Entity>();
                e2 = collision.GetComponentInParent<Entity>();
                e = (e1 == null ? e2 : e1);
                if (e != null)
                {
                    e.Hit(Damage);

                    if (explodeOnImpact)
                        CreateExplosion(collision);
                }
                else
                {
                    UnityEngine.Debug.LogErrorFormat("GameObject '{0}'(parent:{1}, active:{2}) should have a IHitteble in it's hierarchy!",
                        collision.gameObject.name, collision.gameObject.transform.parent, collision.gameObject.activeInHierarchy);
                }
                Destroy();
            }
        }

        void FixedUpdate()
        {
            if (!gameObject.activeInHierarchy)
                return;
            transform.position += transform.right * speed * Time.fixedDeltaTime;
        }

        void CreateExplosion(Collider2D original)
        {
            AppServices.Instance.AudioManager.SoundEffectsManager.PlaySound(SoundEffects.Explosion);

            pooledG = AppServices.Instance.PoolManager.GetPooledPrefabTimed(explosion.Prefab, explosion.Duration);
            pooledG.transform.position = transform.position;
            //pooledG.transform.position = transform.position;

            hits = Physics2D.CircleCastAll(transform.position, explosion.Radius, Vector2.zero, 0, explosion.Mask);
            foreach (var hit in hits)
            {
                if (hit.collider == original || !hit.collider.gameObject.CompareTag(TargetTag))
                    continue;

                e1 = hit.collider.GetComponentInChildren<Entity>();
                e2 = hit.collider.GetComponentInParent<Entity>();
                e = (e1 == null ? e2 : e1);
                if (e != null)
                {
                    dis = Vector2.Distance(hit.point, transform.position);
                    diff = explosion.Radius / dis;
                    dmg = explosion.Damage - (explosion.Damage * diff);

                    //Debug.LogFormat("Dealing damage({0}) to {1}|distance={2}", dmg, c.gameObject.name, dis);

                    e.Hit((int)(dmg < 0 ? 0 : dmg));
                }
                else
                {
                    UnityEngine.Debug.LogError(hit.collider.gameObject.name + " should have a IHitteble in it's hierarchy!");
                }
            }
        }

        void Destroy()
        {
            CancelInvoke();

            if (!AppServices.Instance.PoolManager.DeactivatePrefab(gameObject))
            {
                UnityEngine.Debug.LogErrorFormat("GameObject {0} is acting as PooledPrefab, object was destroyed!", gameObject.name);
                Destroy(gameObject);
            }
        }
    }
}