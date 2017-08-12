using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

[Serializable]
public class PlayerEffects
{
    [SerializeField] Renderer renderer;
    
    [SerializeField] Color chargedColor;
    [SerializeField] float flashDuration;

    [SerializeField] float minDif;

    DelayedCall flashCall = new DelayedCall();

    PerlinShake perlinShake;

    bool isFlashing;
    bool isFlashingLastFrame;

    public void Start()
    {
        perlinShake = Camera.main.GetComponent<PerlinShake>();
        perlinShake.testProjection = true;

    }

    public void Update()
    {
        if (isFlashing && !isFlashingLastFrame)
            Flash(false);
        else if (!isFlashing && isFlashingLastFrame)
            flashCall.Canceled = true;

        isFlashingLastFrame = isFlashing;

    }

    void Flash(bool b)
    {
        Flash(b, chargedColor);
    }
    void Flash(bool b, Color col)
    {
        isFlashing = true;
        if (b)
            SetColor(col);
        else
            SetColor(Color.black);
        
        flashCall.CallBack = () => { Flash(!b); };
        flashCall.Delay = flashDuration;
        flashCall.Canceled = false;
        Services.Instance.StaticCoroutines.Invoke(flashCall);
    }
    void SetColor(Color col)
    {
        renderer.materials[0].SetColor("_EmissionColor", col);
    }

    public void SetTransition(float t)
    {
        #region Gfx

            if (!isFlashing)
                SetColor(Color.Lerp(Color.black, chargedColor, t));
            if (t > 0.95f)
                isFlashing = true;
            else
                isFlashing = false;

        #endregion
    }

    public void HitEffect()
    {
        perlinShake.testRotation = true;
        perlinShake.testRotation = true;
    }
}
