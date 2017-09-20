using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MovementEffects;

namespace Aponi
{
    [Serializable]
    public class GameManager
    {
        public PlayerComponent Player;

        public bool IsPlaying { get; private set; }

        public Action OnGameStart;
        public Action OnGameOver;

        public GameObject ProjectilesField;

        public void Initialize()
        {
            OnGameStart += Start;
            OnGameOver += () => { Timing.Instance.CallDelayedOnInstance(3, End); };

            //OnGameStart += () => { Debug.Log("GameManager_OnGameStart"); };
            //OnGameOver += () => { Debug.Log("GameManager_OnGameOver"); };

            AppServices.Instance.AppInput.AnyKeyDown += StartGame;

            End();
        }

        void Start()
        {
            IsPlaying = true;
            GameServices.Instance.GameUIManager.TitleScreen.SetActive(false);
        }
        void End()
        {
            IsPlaying = false;
            GameServices.Instance.GameUIManager.TitleScreen.SetActive(true);
        }

        public void StartGame()
        {
            if (!IsPlaying)
                OnGameStart?.Invoke();
        }

        public void GameOver()
        {
            if (IsPlaying)
                OnGameOver?.Invoke();
        }

        public void ResetLevel(float delay)
        {
            Timing.RunCoroutine(ResetLevelDelayed(delay));
        }
        public void ResetLevel()
        {
            Timing.RunCoroutine(ResetLevelDelayed(0));
        }

        IEnumerator<float> ResetLevelDelayed(float delay)
        {
            yield return Timing.WaitForSeconds(delay);
            AppServices.Instance.SceneManager.ReloadCurrentScene();
        }
    }
}