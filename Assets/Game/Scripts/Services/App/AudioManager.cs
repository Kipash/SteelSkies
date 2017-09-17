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

    public void Initialize()
    {
        SoundEffectsManager.Initialize();
        AudioService.Initialize();
        WeatherSFXManager.Initialize();

        WeatherSFXManager.CurrentWeather = WeatherType.Rain;
        MusicManager.Reset();
        MusicManager.IsPaused = false;
    }
}
