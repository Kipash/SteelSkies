using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
//using MovementEffects;

namespace Aponi
{

    [Serializable]
    public class EnemyEffects
    {
        [SerializeField] GameObject gameObject;
        [SerializeField] bool GetRenderers;
        [SerializeField] Renderer[] manualRenderers;
        [SerializeField] Color flashColor;
        [SerializeField] float flashDuration = 0.1f;

        [SerializeField] bool disable;

        [Header("Die effects")]
        [SerializeField]
        PooledPrefabs explosion;
        [SerializeField] SoundEffects sfx;

        Renderer[] renderers;
        Coroutine currCoroutine;

        int i;
        GameObject g;

        public void Deactivate()
        {
            if(GameServices.Initialize && currCoroutine != null)
                GameServices.Instance.StopCoroutine(currCoroutine);
            currCoroutine = null;
        }

        public void Start()
        {
            //Timing.Instance.AddTag(EnemyEffectsTag, false);
            if (renderers == null)
            {
                if (GetRenderers)
                    renderers = gameObject.GetComponentsInChildren<Renderer>();
                else
                    renderers = manualRenderers;
            }
            else
            {
                for (i = 0; i < renderers.Length; i++)
                {
                    renderers[i].materials[0].SetColor("_EmissionColor", Color.black);
                }
            }
        }

        public void HitEffect()
        {
            if (disable)
                return;
            GameServices.Instance.StartCoroutine(Flash());
        }

        public void DieEffect(Vector3 exploPos)
        {
            if (disable)
                return;

            AppServices.Instance.AudioManager.SoundEffectsManager.PlaySound(sfx);

            g = AppServices.Instance.PoolManager.GetPooledPrefabTimed(explosion, 3);
            g.transform.position = exploPos;
        }

        IEnumerator Flash()
        {
            foreach (var r in renderers)
                r.materials[0].SetColor("_EmissionColor", flashColor);
            yield return new WaitForSeconds(0.1f);
            foreach (var r in renderers)
                r.materials[0].SetColor("_EmissionColor", Color.black);
        }
    }
}