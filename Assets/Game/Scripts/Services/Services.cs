using UnityEngine;
using System.Collections;

public class Services : MonoBehaviour
{
    public static Services Instance;
    [Header(" - StaticCoroutines - ")]
    public StaticCoroutines StaticCoroutines;
    [Header(" - MusicManager - ")]
    public MusicManager MusicManager;

    void Start()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        MusicManager.IsPaused = false;
    }
}
