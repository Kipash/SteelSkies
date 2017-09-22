using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Audio;
//using MovementEffects;
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

        Coroutine currCoroutine;

        AudioClip nextClip;
        int currClipIndex;
        float currentDelay;
        float pitch;

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
                AppServices.Instance.StopCoroutine(currCoroutine);
            }
            else
                UnityEngine.Debug.LogWarning("No music is playing, cant be stoped!");
        }
        void StartMusic()
        {
            if (audioSource.clip != null)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.UnPause();
                    currentDelay = (audioSource.clip.length) - audioSource.time;
                    AppServices.Instance.StartCoroutine(NextTrack());
                }
            }
            else
                AppServices.Instance.StartCoroutine(NextTrack());

        }
        IEnumerator NextTrack()
        {
            yield return new WaitForSeconds(currentDelay);

            if (clips.Length != 0)
            {
                audioSource.clip = (nextClip == null ? clips[0] : clips[currClipIndex]);
                audioSource.Play();

                currClipIndex++;
                if (currClipIndex >= clips.Length)
                    currClipIndex = 0;
                nextClip = clips[currClipIndex];

                if (group.audioMixer.GetFloat("MusicPitch", out pitch))
                    currentDelay = (audioSource.clip.length / pitch) - audioSource.time;
                else
                    currentDelay = audioSource.clip.length - audioSource.time;

                AppServices.Instance.StartCoroutine(NextTrack());
            }
        }
    }
}