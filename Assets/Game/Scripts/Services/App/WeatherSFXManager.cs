using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MovementEffects;

[Serializable]
public class WeatherSFXManager
{
    [SerializeField] WeatherSoundEffect[] clips;

    AudioSource source;
    AudioSource audioSource
    {
        get
        {
            if (source == null)
            {
                source = AppServices.Instance.AudioManager.AudioService.GetPermanentSource();
            }
            return source;
        }
    }

    Dictionary<WeatherType, WeatherSoundEffect> soundEffects = new Dictionary<WeatherType, WeatherSoundEffect>();

    WeatherType currentWeather;
    public WeatherType CurrentWeather
    {
        get
        {
            return currentWeather;
        }
        set
        {
            currentWeather = value;
            ChangeWeather();
        }
    }

    public void Initialize()
    {
        soundEffects = clips.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.First());
        Timing.CallDelayed(1, () => { Timing.RunCoroutine(PlayRandomSFX()); });
    }


    void ChangeWeather()
    {
        var fx = soundEffects[currentWeather];

        audioSource.Stop();

        audioSource.loop = fx.Loop;
        audioSource.clip = fx.Sound.Clip;
        audioSource.outputAudioMixerGroup = fx.Sound.Group;
        audioSource.volume = fx.Sound.Volume;

        audioSource.Play();
    }

    IEnumerator<float> PlayRandomSFX()
    {
        var delay = soundEffects[CurrentWeather].RandomSFXDelay + UnityEngine.Random.Range(0, soundEffects[CurrentWeather].RandomSFXRange);
        yield return Timing.WaitForSeconds(delay);

        var sfx = soundEffects[CurrentWeather]
            .RandomSounds[UnityEngine.Random.Range(0, soundEffects[CurrentWeather].RandomSounds.Length)];
        Debug.Log("playing: " + soundEffects[CurrentWeather].Name);
        AppServices.Instance.AudioManager.AudioService.Play(sfx.Group, sfx.Clip, sfx.Volume);


        Timing.RunCoroutine(PlayRandomSFX());
    }
}
