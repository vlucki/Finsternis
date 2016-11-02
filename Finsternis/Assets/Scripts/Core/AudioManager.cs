namespace Finsternis {
    using UnityEngine;
    using System.Collections;
    using UnityEngine.Audio;
    using System;
    using UnityQuery;
    using System.Collections.Generic;

    public class AudioManager : MonoBehaviour
    {
        [Serializable]
        public struct AudioMixers
        {
            public AudioMixer bgmMixer;
            public AudioMixer bsgMixer;
            public AudioMixer sfxMixer;
        }

        [SerializeField]
        private AudioMixers audioMixers;

        [SerializeField]
        [Range(.01f, 1f)]
        private float fadeInLerpAmount = .05f;

        [SerializeField]
        [Range(.01f, 1f)]
        private float fadeOutLerpAmount = .05f;

        [SerializeField]
        private AudioSource bgmSourceA;
        [SerializeField]
        private AudioSource bgmSourceB;

        [SerializeField]
        private AudioSource bsgSourceA;

        [SerializeField]
        private AudioSource bsgSourceB;

        private Dictionary<AudioSource, Coroutine> transitions;

        [Space]
        public AudioClip toPlayA;
        public AudioClip toPlayB;
        
        void Awake()
        {
            this.transitions = new Dictionary<AudioSource, Coroutine>();
        }

        void Start()
        {
            PlayBGM(toPlayA);
            this.CallDelayed(10, TestXFade);
        }

        private void TestXFade()
        {
            PlayBGM(toPlayB);
        }

        public void PlayBGM(AudioClip newBgm)
        {
            if (bgmSourceA.isPlaying || bgmSourceB.isPlaying)
            {
                CrossFade(bgmSourceA, bgmSourceB, newBgm, 1);
            }
            else
            {
                bgmSourceA.clip = newBgm;
                FadeIn(bgmSourceA, 1);
            }
        }

        private void CrossFade(AudioSource sourceA, AudioSource sourceB, AudioClip newClip, float newClipVolume)
        {
            var currentlyPlaying = sourceA.isPlaying ? sourceA : sourceB;
            var toBePlayed = sourceA.isPlaying ? sourceB : sourceA;

            toBePlayed.clip = newClip;
            toBePlayed.volume = 0;

            FadeOut(currentlyPlaying);
            FadeIn(toBePlayed, newClipVolume);

        } 

        public void FadeIn(AudioSource source, float targetVolume, Action callback = null)
        {
            StopTransition(source);
            this.transitions.Add(source, StartCoroutine(_FadeIn(source, targetVolume, callback)));
        }

        public void FadeOut(AudioSource source, Action callback = null)
        {
            StopTransition(source);
            this.transitions.Add(source, StartCoroutine(_FadeOut(source, callback)));
        }

        private void StopTransition(AudioSource source)
        {
            Coroutine transition;
            if (this.transitions.TryGetValue(source, out transition))
            {
                this.StopCoroutine(transition);
                this.transitions.Remove(source);
            }
        }

        private IEnumerator _FadeIn(AudioSource source, float targetVolume, Action callback = null)
        {
            source.volume = 0;
            source.Play();
            while (source.volume < targetVolume * .99f)
            {
                source.volume = Mathf.Lerp(source.volume, targetVolume, this.fadeInLerpAmount);
                yield return null;
            }
            source.volume = targetVolume;

            if (callback != null)
                callback();
        }

        private IEnumerator _FadeOut(AudioSource source, Action callback = null)
        {
            while (source.volume > .01f)
            {
                source.volume = Mathf.Lerp(source.volume, 0, this.fadeOutLerpAmount);
                yield return null;
            }
            source.volume = 0;
            source.Stop();

            if (callback != null)
                callback();
        }
    }
}
