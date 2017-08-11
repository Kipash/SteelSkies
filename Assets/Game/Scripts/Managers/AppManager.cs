using UnityEngine;
using System.Collections;

public class AppManager : MonoBehaviour
{
    public static AppManager Instance;
    [SerializeField] bool AutoSwitchScene;

    void Start()
    { 
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        if (Application.loadedLevel == 0 && AutoSwitchScene)
            Application.LoadLevel(Application.loadedLevel + 1);

        CurrentBinds.SetDefaults();
        DontDestroyOnLoad(gameObject);
    }
}
