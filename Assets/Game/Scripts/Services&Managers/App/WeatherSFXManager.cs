using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using MovementEffects;

namespace SteelSkies
{
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

        WeatherSoundEffect fx;
        float delay;
        Sound sound;

        public void Initialize()
        {
            soundEffects = clips.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.First());
            AppServices.Instance.StartCoroutine(CommonCoroutine.CallDelay(() => { AppServices.Instance.StartCoroutine(PlayRandomSFX()); }, 1));
        }
        
        void ChangeWeather()
        {
            fx = soundEffects[currentWeather];

            audioSource.Stop();

            audioSource.loop = fx.Loop;
            audioSource.clip = fx.Sound.Clip;
            audioSource.outputAudioMixerGroup = fx.Sound.Group;
            audioSource.volume = fx.Sound.Volume;

            audioSource.Play();
        }

        IEnumerator PlayRandomSFX()
        {
            delay = soundEffects[CurrentWeather].RandomSFXDelay + UnityEngine.Random.Range(0, soundEffects[CurrentWeather].RandomSFXRange);
            yield return new WaitForSeconds(delay);

            sound = soundEffects[CurrentWeather]
                .RandomSounds[UnityEngine.Random.Range(0, soundEffects[CurrentWeather].RandomSounds.Length)];
            //UnityEngine.Debug.Log("playing: " + soundEffects[CurrentWeather].Name);
            AppServices.Instance.AudioManager.AudioService.Play(sound.Group, sound.Clip, sound.Volume);


            AppServices.Instance.StartCoroutine(PlayRandomSFX());
        }
    }
}