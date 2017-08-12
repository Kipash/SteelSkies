using UnityEngine;
using System.Collections;
using App.Player.Services;
using System.Collections.Generic;
using System.Linq;

public class Services : MonoBehaviour
{
    public static Services Instance;


    [Header(" - StaticCoroutines - ")]
    public StaticCoroutines StaticCoroutines;

    [Header(" - MusicManager - ")]
    public MusicManager MusicManager;

    [Header(" - AppInput - ")]
    public AppInput AppInput = new AppInput();

    [Header(" - PoolManager - ")]
    public PrefabPoolManager PoolManager;

    void Start()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        MusicManager.IsPaused = false;

        PoolManager.Start();
    }

    List<GameObject> objs = new List<GameObject>();

    private void Update()
    {
        AppInput.CheckInput();

        if(Input.GetKeyDown(KeyCode.F1))
        {
            var g = PoolManager.GetPooledPrefab(PooledPrefabs.Bullet);
            g.transform.parent = null;
            objs.Add(g);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (objs.Count != 0)
            {
                var g = objs.First();
                objs.Remove(g);
                PoolManager.DeactivatePrefab(g);
            }
        }
    }
}
