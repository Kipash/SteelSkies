using UnityEngine;
using System.Collections;
using App.Player.Services;
using System.Collections.Generic;
using System.Linq;
using System;

public class Services : MonoBehaviour
{
    public static Services Instance;


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

    void Awake()
    {
        var t = DateTime.Now;


        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        
        PoolManager.Start();

        AudioService.Start();

        MusicManager.IsPaused = false;


        print("All services loaded in " + (DateTime.Now - t).TotalMilliseconds + " ms.");
    }

    private void Update()
    {
        AppInput.CheckInput();
    }
}
