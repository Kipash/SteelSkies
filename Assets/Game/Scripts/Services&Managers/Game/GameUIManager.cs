using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

namespace Aponi
{
    [Serializable]
    public class GameUIManager
    {
        public Transform BannerRoot;

        public GameObject TitleScreen;
        public GameObject GameOver;
        public GameObject NextWave;

        public Dial GameScore;
        public Dial PlayerHealth;

        public Text TextPrefab;

        public void ShowDialog(Vector3 worldPosition, string msg, float time = 1)
        {
            var t = AppServices.Instance.PoolManager.GetPooledPrefabTimed(PooledPrefabs.UITextBubble, time);
            var banner = t.GetComponentInChildren<Text>();

            t.transform.SetParent(BannerRoot);
            banner.text = msg;

            var trans = t.GetComponent<RectTransform>();
            //Debug.LogFormat("WTS({0}) = {1}", worldPosition, Camera.main.WorldToScreenPoint(worldPosition));
            trans.position = Camera.main.WorldToScreenPoint(worldPosition);
        }
    }
}