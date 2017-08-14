using UnityEngine;
using System.Collections;
using App.Player.Services;
using System.Collections.Generic;
using System.Linq;
using System;

public class AppServices : MonoBehaviour
{
    public static AppServices Instance;


    [Header(" - StaticCoroutines - ")]
    public StaticCoroutines StaticCoroutines;

    [Header(" - MusicManager - ")]
    public MusicManager MusicManager;

    [Header(" - AudioService - ")]
    public AudioService AudioService;

    [Header(" - AppInput - ")]
    public AppInput AppInput = new AppInput();


    [Header(" - PoolManager - ")]
    public PrefabPoolManager PoolManager;

    void Start()
    {
        var t = DateTime.Now;


        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        
        PoolManager.Start();

        AudioService.Start();

        MusicManager.IsPaused = false;

        AppManager.Instance.OnLoadLevel += StaticCoroutines.StopAllCoroutines;

        var diff = (DateTime.Now - t);
        Debug.LogFormat("All App services loaded in {0} ms ({1} tics)", diff.TotalMilliseconds, diff.Ticks);
    }

    private void Update()
    {
        AppInput.CheckInput();
    }
}
