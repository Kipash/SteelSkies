using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Audio;
using MovementEffects;

[Serializable]
public class AudioService
{
    [SerializeField] GameObject AudioSourceRoot;
    
    [SerializeField] float preBuildSources;
    [SerializeField] AnimationCurve volumeRollOff;

    
    List<AudioSource> allSources = new List<AudioSource>();
    List<AudioSource> availableSources = new List<AudioSource>();

    public void Initialize()
    {
        for (int i = 0; i < preBuildSources; i++)
        {
            var s = AddSource();
            s.enabled = false;
        }

        availableSources = allSources.ToList();
    }

    public AudioSource GetPermanentSource()
    {
        return GetSource();
    }

    public void Play(Sound sound)
    {
        Play(sound.Group, sound.Clip, sound.Volume);
    }

    public void Play(AudioMixerGroup group, AudioClip clip, float volume)
    {
        var s = GetSource();

        s.outputAudioMixerGroup = group;
        s.clip = clip;
        s.Play();
        s.volume = volume;


        Timing.Instance.CallDelayedOnInstance(clip.length, () => { DeactivateSource(s); });
    }

    void DeactivateSource(AudioSource s)
    {
        s.enabled = false;
        s.clip = null;
        s.volume = 1;
        s.outputAudioMixerGroup = null;
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
