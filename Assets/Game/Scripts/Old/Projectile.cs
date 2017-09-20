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
            if (collision.CompareTag(TargetTag) && collision.gameObject.activeInHierarchy)
            {
                var c1 = collision.GetComponentInChildren<Entity>();
                var c2 = collision.GetComponentInParent<Entity>();
                var c = (c1 == null ? c2 : c1);
                if (c != null)
                {
                    c.Hit(Damage);

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

        void CreateExplosion(Collider2D original)
        {
            AppServices.Instance.AudioManager.SoundEffectsManager.PlaySound(SoundEffects.Explosion);

            var g = AppServices.Instance.PoolManager.GetPooledPrefabTimed(explosion.Prefab, explosion.Duration);
            g.transform.position = transform.position;
            g.transform.position = transform.position;

            hits = Physics2D.CircleCastAll(transform.position, explosion.Radius, Vector2.zero, 0, explosion.Mask);
            foreach (var hit in hits)
            {
                if (hit.collider == original || !hit.collider.gameObject.CompareTag(TargetTag))
                    continue;

                var c1 = hit.collider.GetComponentInChildren<Entity>();
                var c2 = hit.collider.GetComponentInParent<Entity>();
                var c = (c1 == null ? c2 : c1);
                if (c != null)
                {
                    var dis = Vector2.Distance(hit.point, transform.position);
                    var diff = explosion.Radius / dis;
                    var dmg = explosion.Damage - (explosion.Damage * diff);

                    //Debug.LogFormat("Dealing damage({0}) to {1}|distance={2}", dmg, c.gameObject.name, dis);

                    c.Hit((int)(dmg < 0 ? 0 : dmg));
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