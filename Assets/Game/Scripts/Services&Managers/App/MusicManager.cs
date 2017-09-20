using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Audio;
using MovementEffects;
using System.Collections.Generic;

namespace Aponi
{
    [Serializable]
    public class MusicManager
    {
        AudioSource source;
        AudioSource audioSource
        {
            get
            {
                if (source == null)
                {
                    source = AppServices.Instance.AudioManager.AudioService.GetPermanentSource();
                    source.outputAudioMixerGroup = group;
                }
                return source;
            }
        }
        [SerializeField] AudioMixerGroup group;
        [SerializeField] AudioClip[] clips;

        string musicTag = "MusicTag";

        AudioClip nextClip;
        int currClipIndex;
        float currentDelay;

        public void Reset()
        {
            clips.Shuffle();
        }

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
                Timing.KillCoroutines(musicTag);
            }
            else
                UnityEngine.Debug.LogWarning("No music is playing, cant be stoped!");
        }
        void StartMusic()
        {
            Timing.Instance.AddTag(musicTag, true);
            if (audioSource.clip != null)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.UnPause();
                    currentDelay = (audioSource.clip.length) - audioSource.time;
                    Timing.RunCoroutine(NextTrack(), musicTag);
                }
            }
            else
                Timing.RunCoroutine(NextTrack(), musicTag);
        }
        IEnumerator<float> NextTrack()
        {
            yield return Timing.WaitForSeconds(currentDelay);

            if (clips.Length != 0)
            {
                audioSource.clip = (nextClip == null ? clips[0] : clips[currClipIndex]);
                audioSource.Play();

                currClipIndex++;
                if (currClipIndex >= clips.Length)
                    currClipIndex = 0;
                nextClip = clips[currClipIndex];

                float pitch;
                if (group.audioMixer.GetFloat("MusicPitch", out pitch))
                    currentDelay = (audioSource.clip.length / pitch) - audioSource.time;
                else
                    currentDelay = audioSource.clip.length - audioSource.time;

                Timing.RunCoroutine(NextTrack(), musicTag);
            }
        }
    }
}