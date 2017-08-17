using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Audio;

[Serializable]
public class WeatherSoundEffect
{
    
    public string Name;
    public bool Loop;
    public Sound Sound;
    public WeatherType Type;
    
    [Header("Random sounds")]
    public Sound[] RandomSounds;
    public float RandomSFXDelay;
    public float RandomSFXRange;

}
