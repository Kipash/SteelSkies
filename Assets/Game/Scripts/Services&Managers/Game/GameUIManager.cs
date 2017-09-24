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

        GameObject g;
        Text t;
        RectTransform rt;

        public void ShowDialog(Vector3 worldPosition, string msg, float time = 1)
        {
            g = AppServices.Instance.PoolManager.GetPooledPrefabTimed(PooledPrefabs.UITextBubble, time);
            t = g.GetComponentInChildren<Text>();

            g.transform.SetParent(BannerRoot);
            t.text = msg;

            rt = g.GetComponent<RectTransform>();
            //Debug.LogFormat("WTS({0}) = {1}", worldPosition, Camera.main.WorldToScreenPoint(worldPosition));
            g.transform.position = Camera.main.WorldToScreenPoint(worldPosition);
        }
    }
}