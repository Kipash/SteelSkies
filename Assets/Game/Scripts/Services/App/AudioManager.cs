using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class AudioManager
{
    [Header(" - SoundEffectsManager - ")]
    public SoundEffectsManager SoundEffectsManager;

    [Header(" - WeatherSFXManager - ")]
    public WeatherSFXManager WeatherSFXManager;

    [Header(" - MusicManager - ")]
    public MusicManager MusicManager;

    [Header(" - AudioService - ")]
    public AudioService AudioService;

    public void Start()
    {
        SoundEffectsManager.Start();
        AudioService.Start();
        WeatherSFXManager.Start();

        WeatherSFXManager.CurrentWeather = WeatherType.Rain;
        MusicManager.Reset();
        MusicManager.IsPaused = false;
    }
}
