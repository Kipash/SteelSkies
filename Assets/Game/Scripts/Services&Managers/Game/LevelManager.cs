using UnityEngine;
using System;
using System.Collections;

namespace SteelSkies
{
    [Serializable]
    public class LevelManager
    {
        [SerializeField] int baseXP;
        int currentMultiplier = 1;
        int currentXP;
        public int CurrentLevel
        {
            get
            {
                return currentMultiplier;
            }
        }
        public event Action<int> OnLevelUp;
        
        public void Initialize()
        {
            GameServices.Instance.GameUIManager.LevelSlider.value = currentXP / (baseXP * currentMultiplier / 100) / 1000;
            OnLevelUp += LevelUp;
            GameServices.Instance.GameManager.OnGameStart += Reset;
            LevelUp(0);
        }

        public void AddXP(int amount)
        {
            if (currentXP + amount >= baseXP * currentMultiplier)
            {
                int overdraw = (currentXP + amount) - (baseXP * currentMultiplier);
                currentMultiplier++;
                currentXP = overdraw;

                OnLevelUp?.Invoke(CurrentLevel);
            }
            else
                currentXP += amount;

            GameServices.Instance.GameUIManager.LevelSlider.value = currentXP;
        }
        void Reset()
        {
            currentMultiplier = 1;
            currentXP = 0;
        }
        void LevelUp(int i)
        {
            GameServices.Instance.GameUIManager.LevelSlider.maxValue = baseXP * currentMultiplier;
        }
    }
}