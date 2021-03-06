﻿using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEngine.UI;
//using MovementEffects;
using System.Collections.Generic;

namespace SteelSkies
{

    [Serializable]
    public class PlayerEffects
    {
        internal enum EffectMode { none, Charging, Damage }

        [SerializeField] Transform transform;
        [SerializeField] Renderer renderer;

        [SerializeField] Color chargedColor;
        [SerializeField] float flashDuration;

        [SerializeField] float minDif;

        [Header("Die effects")]
        [SerializeField]
        PooledPrefabs explosion;
        [SerializeField] SoundEffects sfx;

        string flashTag = "PlayerFlash"; //TODO: Random ?
        float currentDelay;

        PerlinShake perlinShake;

        bool isFlashing;
        bool isFlashingLastFrame;

        GameObject g;

        EffectMode mode;

        [SerializeField] bool disabled;
        public bool Disabled
        {
            get { return disabled; }
            set { disabled = value; }
        }

        public void Start()
        {
            //Timing.Instance.AddTag(flashTag, false);

            AppServices.Instance.SceneManager.OnSceneChanged += SceneManager_OnSceneChanged;

            perlinShake = Camera.main.GetComponent<PerlinShake>();

        }

        private void SceneManager_OnSceneChanged(Scenes newScene, Scenes oldScene)
        {
            perlinShake.StopAllCoroutines();
            perlinShake.ResetCameraEffects();

            AppServices.Instance.SceneManager.OnSceneChanged -= SceneManager_OnSceneChanged;
        }

        public void Update()
        {
            if (!Disabled)
            {
                if (isFlashing && !isFlashingLastFrame)
                    GameServices.Instance.StartCoroutine(Flash(false));
                else if (!isFlashing && isFlashingLastFrame)
                    GameServices.Instance.StartCoroutine(flashTag);

                isFlashingLastFrame = isFlashing;
            }
            else
                SetColor(Color.black);
        }

        IEnumerator Flash(bool b)
        {
            GameServices.Instance.StartCoroutine(Flash(b, chargedColor));
            yield return new WaitForEndOfFrame();
        }
        IEnumerator Flash(bool b, Color col)
        {
            yield return new WaitForSeconds(currentDelay);

            isFlashing = true;
            if (b)
                SetColor(col);
            else
                SetColor(Color.black);

            currentDelay = flashDuration;
            GameServices.Instance.StartCoroutine(Flash(!b));
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

                g = AppServices.Instance.PoolManager.GetPooledPrefabTimed(explosion, 5);
                g.transform.position = transform.position;

                AppServices.Instance.AudioManager.SoundEffectsManager.PlaySound(sfx);
            }
        }
    }
}