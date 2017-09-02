using MovementEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameServices : MonoBehaviour
{
    public static bool IsMainManagerPresent;
    public static GameServices Instance { get; private set; }
    
    [Header(" - ChallengeManager - ")]
    public ChallengeManager ChallengeManager;

    [Header(" - GameManager - ")]
    public GameManager GameManager;

    [Header(" - WayPointManager - ")]
    public WayPointManager WayPointManager;

    [Header("Temp")]
    [SerializeField] Text scoreText;

    void Awake()
    {
        IsMainManagerPresent = FindObjectOfType<AppManager>() != null;
        if (!IsMainManagerPresent)
            return;

        AppManager.Instance.OnLoadLevel += GameServices_OnLoadLevel;

        var t = DateTime.Now;

        Instance = this;

        ChallengeManager.Start();
        WayPointManager.Start();

        var diff = (DateTime.Now - t);
        Debug.LogFormat("All Game services loaded in {0} ms ({1} tics)", diff.TotalMilliseconds, diff.Ticks);
    }

    private void GameServices_OnLoadLevel()
    {
        Timing.KillCoroutines(ChallengeManager.SpawningTag);
        AppManager.Instance.OnLoadLevel -= GameServices_OnLoadLevel;
    }

    private void Update()
    {
        scoreText.text = string.Format("{0}\nBest:{1}", ChallengeManager.Score, ChallengeManager.BestScore);
    }
}
