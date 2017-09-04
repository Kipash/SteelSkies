using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

[Serializable]
public class SoundEffectsManager
{
    [SerializeField] SoundEffect[] clips;
    Dictionary<SoundEffects, SoundEffect> effects = new Dictionary<SoundEffects, SoundEffect>();

    public void Initialize()
    {
        effects = clips.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.First());
    }

    public void PlaySound(SoundEffects sound)
    {
        if (!effects.ContainsKey(sound))
        {
            Debug.LogErrorFormat("Sound {0} isnt registered, but still trying to be played!", sound);
            return;
        }
        var sfx = effects[sound];
        AppServices.Instance.AudioManager.AudioService.Play(sfx.Sound.Group, sfx.Sound.Clip, sfx.Sound.Volume);
    }
}
