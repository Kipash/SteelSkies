﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MovementEffects;

[Serializable]
public class EnemyEffects
{
    [SerializeField] GameObject gameObject;
    [SerializeField] bool GetRenderers;
    [SerializeField] Renderer[] manualRenderers;
    [SerializeField] Color flashColor;
    [SerializeField] float flashDuration = 0.1f;

    Renderer[] renderers;

    public const string EnemyEffectsTag = "EnemyEffectsTag";

    public void Start()
    {
        Timing.Instance.AddTag(EnemyEffectsTag, false);

        if (GetRenderers)
            renderers = gameObject.GetComponentsInChildren<Renderer>();
        else
            renderers = manualRenderers;
    }

    public void HitEffect()
    {
        Timing.Instance.RunCoroutineOnInstance(Flash(), EnemyEffectsTag);
    }

    public void DieEffect(Vector3 exploPos)
    {
        AppServices.Instance.AudioManager.SoundEffectsManager.PlaySound(SoundEffects.Explosion);

        var explosion = AppServices.Instance.PoolManager.GetPooledPrefabTimed(PooledPrefabs.SmallExplsion, 3);
        explosion.transform.position = exploPos;
    }

    IEnumerator<float> Flash()
    {
        foreach(var r in renderers)
            r.materials[0].SetColor("_EmissionColor", flashColor);
        yield return Timing.WaitForSeconds(0.1f);
        foreach (var r in renderers)
            r.materials[0].SetColor("_EmissionColor", Color.black);
    }
}
