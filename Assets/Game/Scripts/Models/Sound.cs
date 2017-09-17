using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Audio;

[Serializable]
public class Sound
{
    public AudioClip Clip;
    [Range(0.0f, 1.0f)] public float Volume;
    public AudioMixerGroup Group;
}
