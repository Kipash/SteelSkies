using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SoundEffect
{
    public string Name;
    public AudioClip Clip;
    public SoundEffects Type; 
}

[Serializable]
public class AudioService
{
    [SerializeField] GameObject AudioSourceRoot;
    [SerializeField] SoundEffect[] clips;
    [SerializeField] float preBuildSources;
    [SerializeField] AnimationCurve volumeRollOff;

    Dictionary<SoundEffects, SoundEffect> effects = new Dictionary<SoundEffects, SoundEffect>();
    List<AudioSource> allSources = new List<AudioSource>();
    List<AudioSource> availableSources = new List<AudioSource>();

    public void Start()
    {
        effects = clips.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.First());

        for (int i = 0; i < preBuildSources; i++)
        {
            var s = AddSource();
            s.enabled = false;
        }

        availableSources = allSources.ToList();
    }

    public void PlaySound(SoundEffects sound)
    {
        if (!effects.ContainsKey(sound))
        {
            Debug.LogErrorFormat("Sound {0} isnt registered, but still trying to be played!", sound);
            return;
        }

        Play(effects[sound].Clip);
    }

    public AudioSource GetPermanentSource()
    {
        return GetSource();
    }


    void Play(AudioClip clip)
    {
        var s = GetSource();

        s.clip = clip;
        s.Play();

        var deactivateSource = new DelayedCall();

        deactivateSource.Delay = clip.length;
        deactivateSource.CallBack = () => { DeactivateSource(s); };
        Services.Instance.StaticCoroutines.Invoke(deactivateSource);
    }

    void DeactivateSource(AudioSource s)
    {
        s.enabled = false;
        s.clip = null;
        availableSources.Add(s);
    }

    AudioSource AddSource()
    {
        var s = AudioSourceRoot.AddComponent<AudioSource>();

        s.rolloffMode = AudioRolloffMode.Custom;
        s.SetCustomCurve(AudioSourceCurveType.CustomRolloff, volumeRollOff);

        allSources.Add(s);
        return s;
    }

    AudioSource GetSource()
    {
        AudioSource s;
        if (availableSources.Count == 0)
            s = AddSource();
        else
            s = availableSources.First();

        availableSources.Remove(s);
        s.enabled = true;

        return s;
    }
}
