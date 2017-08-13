using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class MusicManager
{
    AudioSource source;
    AudioSource audioSource
    {
        get
        {
            if (source == null)
                source = Services.Instance.AudioService.GetPermanentSource();
            return source;
        }
    }
        
    [SerializeField] AudioClip[] clips;

    AudioClip nextClip;
    int currClipIndex;

    DelayedCall callBack;

    bool isPaused;
    public bool IsPaused
    {
        get
        {
            return isPaused;
        }
        set
        {
            isPaused = value;
            if (value)
                PauseMusic();
            else
                StartMusic();
        }
    }

    void PauseMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            Services.Instance.StaticCoroutines.CancelInvoke(callBack);
        }
        else
            Debug.LogWarning("No music is playing, cant be stoped!");
    }
    void StartMusic()
    {
        if (audioSource.clip != null)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.UnPause();
                callBack.Delay = audioSource.clip.length - audioSource.time;
                Services.Instance.StaticCoroutines.Invoke(callBack);
            }
        }
        else
            NextTrack();
    }
    void NextTrack()
    {
        if (clips.Length != 0)
        {
            audioSource.clip = (nextClip == null ? clips[0] : clips[currClipIndex]);
            audioSource.Play();

            currClipIndex++;
            if (currClipIndex >= clips.Length)
                currClipIndex = 0;
            nextClip = clips[currClipIndex];

            if (callBack == null)
                callBack = new DelayedCall() { CallBack = NextTrack };

            callBack.Delay = audioSource.clip.length - audioSource.time;
            Services.Instance.StaticCoroutines.Invoke(callBack);
        }
    }
}
