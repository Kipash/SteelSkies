using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class MusicManager
{
    [SerializeField] AudioSource source;
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
        if (source.isPlaying)
        {
            source.Pause();
            Services.Instance.StaticCoroutines.CancelInvoke(callBack);
        }
        else
            Debug.LogWarning("No music is playing, cant be stoped!");
    }
    void StartMusic()
    {
        if (source.clip != null)
        {
            if (!source.isPlaying)
            {
                source.UnPause();
                callBack.Delay = source.clip.length - source.time;
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
            source.clip = (nextClip == null ? clips[0] : clips[currClipIndex]);
            source.Play();

            currClipIndex++;
            if (currClipIndex >= clips.Length)
                currClipIndex = 0;
            nextClip = clips[currClipIndex];

            if (callBack == null)
                callBack = new DelayedCall() { CallBack = NextTrack };

            callBack.Delay = source.clip.length - source.time;
            Services.Instance.StaticCoroutines.Invoke(callBack);
        }
    }
}
