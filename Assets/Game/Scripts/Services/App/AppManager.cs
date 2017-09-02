using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Linq;
using MovementEffects;

public class AppManager : MonoBehaviour
{
    public static AppManager Instance { get; private set; }
    public event Action OnLoadLevel;

    void Awake()
    {
        Debug.ClearDeveloperConsole();

        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        
        CurrentBinds.SetDefaults();
        DontDestroyOnLoad(gameObject);

        /* -- commit ID --

        var dir = new DirectoryInfo(Application.dataPath);

        var path = dir.Parent.FullName + @"\.git\logs\refs\heads\master";
        var lines = File.ReadAllLines(path);

        print("the one: " + lines.Last().Split(' ')[1].Substring(0, 8));

        */

        OnLoadLevel += ResetCoroutine_OnLoadLevel;
    }

    void ResetCoroutine_OnLoadLevel()
    {
        Timing.Instance.KillCoroutines(false);
    }

    public void LoadLevel(int index)
    {
        if(OnLoadLevel != null)
            OnLoadLevel.Invoke();

        Application.LoadLevel(index);
    }
}
