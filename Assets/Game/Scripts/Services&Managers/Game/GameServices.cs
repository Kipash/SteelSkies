﻿//using MovementEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SteelSkies
{
    public class GameServices : MonoBehaviour
    {
        public static GameServices Instance { get; private set; }
        public static bool Initialize;

        [Header(" - ChallengeManager - ")]
        public CahllengeManager ChallengeManager;

        [Header(" - LevelManager - ")]
        public LevelManager LevelManager;

        [Header(" - GameManager - ")]
        public GameManager GameManager;

        [Header(" - WayPointManager - ")]
        public WayPointManager WayPointManager;

        [Header(" - GameUIManager - ")]
        public GameUIManager GameUIManager;



        void Awake()
        {
            if (!AppServices.Initiliazed)
                return;

            Initialize = true;

            AppServices.Instance.SceneManager.OnSceneChanged += SceneManager_OnSceneChanged;

            var t = DateTime.Now;

            Instance = this;

            GameManager.Initialize();

            ChallengeManager.Initialize();
            WayPointManager.Initialize();
            LevelManager.Initialize();

            var diff = (DateTime.Now - t);
            UnityEngine.Debug.LogFormat("All Game services loaded in {0} ms ({1} tics)", diff.TotalMilliseconds, diff.Ticks);
        }

        private void SceneManager_OnSceneChanged(Scenes newScene, Scenes oldScene)
        {
            StopAllCoroutines();
            AppServices.Instance.SceneManager.OnSceneChanged -= SceneManager_OnSceneChanged;
            Initialize = false;
        }
    }
}