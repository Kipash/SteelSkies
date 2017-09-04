using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEngine.UI;
using MovementEffects;
using System.Collections.Generic;

[Serializable]
public class PlayerEffects
{
    internal enum EffectMode { none, Charging, Damage }

    [SerializeField] Renderer renderer;
    
    [SerializeField] Color chargedColor;
    [SerializeField] float flashDuration;

    [SerializeField] float minDif;

    string flashTag = "PlayerFlash"; //TODO: Random ?
    float currentDelay;

    PerlinShake perlinShake;

    bool isFlashing;
    bool isFlashingLastFrame;

    EffectMode mode;

    public bool Disabled;

    public void Start()
    {
        Timing.Instance.AddTag(flashTag, false);

        AppManager.Instance.OnLoadLevel += Effects_OnLoadLevel;
        
        perlinShake = Camera.main.GetComponent<PerlinShake>();
    }

    private void Effects_OnLoadLevel()
    {
        perlinShake.StopAllCoroutines();

        AppManager.Instance.OnLoadLevel -= Effects_OnLoadLevel;
    }

    public void Update()
    {
        if (!Disabled)
        {
            if (isFlashing && !isFlashingLastFrame)
                Timing.Instance.RunCoroutineOnInstance(Flash(false), flashTag);
            else if (!isFlashing && isFlashingLastFrame)
                Timing.Instance.KillCoroutinesOnInstance(flashTag);

            isFlashingLastFrame = isFlashing;
        }
        else
            SetColor(Color.black);
    }

    IEnumerator<float> Flash(bool b)
    {
        Timing.Instance.RunCoroutineOnInstance(Flash(b, chargedColor), flashTag);
        yield return Timing.WaitForOneFrame;
    }
    IEnumerator<float> Flash(bool b, Color col)
    {
        yield return Timing.WaitForSeconds(currentDelay);

        isFlashing = true;
        if (b)
            SetColor(col);
        else
            SetColor(Color.black);

        currentDelay = flashDuration;
        Timing.Instance.RunCoroutineOnInstance(Flash(!b), flashTag);
    }
    void SetColor(Color col)
    {
        renderer.materials[0].SetColor("_EmissionColor", col);
    }

    public void SetTransition(float t)
    {
        #region Gfx
        if (!Disabled)
        {

            if (!isFlashing)
                SetColor(Color.Lerp(Color.black, chargedColor, t));
            if (t > 0.95f)
                isFlashing = true;
            else
                isFlashing = false;

         
        }
        #endregion
    }

    public void HitEffect()
    {
        if (!Disabled)
        {
            perlinShake.testRotation = true;
            perlinShake.testRotation = true;
        }
    }

    public void DieEffect()
    {
        if (!Disabled)
        {
            perlinShake.testProjection = true;
        }
    }
}
