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
            public AudioMixer bgsMixer;
            public AudioMixer sfxMixer;
        }

        [Serializable]
        public struct BGMSources
        {
            public AudioSource A;
            public AudioSource B;
        }

        [Serializable]
        public struct BGSSources
        {
            public AudioSource A;
            public AudioSource B;
        }


        [SerializeField]
        private AudioMixers audioMixers;

        [SerializeField]
        private GameObject SFXSourcePrefab;

        [SerializeField]
        private BGMSources BGM;

        [SerializeField]
        private BGSSources BGS;

        [SerializeField]
        [Range(.01f, 1f)]
        private float fadeInLerpAmount = .05f;

        [SerializeField]
        [Range(.01f, 1f)]
        private float fadeOutLerpAmount = .05f;

        [SerializeField][Range(1, 50)]
        private int maxSFXSources = 10;
        
        private AudioClip lastBGS;

        /// <summary>
        /// Similar to an "Undo" command, replays the last if any, and discards it. If there was no last BGS, it simply stops the one currently played, if any.
        /// </summary>
        internal void ReplayLastBGS()
        {
            if (this.lastBGS)
            {
                PlayBGS(this.lastBGS);
                this.lastBGS = null;
            }
            else
            {
                AudioSource activeBGS = this.BGS.A.isPlaying ? this.BGS.A : this.BGS.B.isPlaying ? this.BGS.B : null;
                if (activeBGS)
                {
                    this.FadeOut(activeBGS);
                }
            }
        }

        private Dictionary<AudioSource, Coroutine> transitions;

        private List<AudioSource> sfxSourcesPool;
        void Awake()
        {
            this.transitions = new Dictionary<AudioSource, Coroutine>();
            this.sfxSourcesPool = new List<AudioSource>(this.maxSFXSources);
            SetBGMVolume(PlayerPrefs.GetFloat("bgmVolume", 1));
            SetSFXVolume(PlayerPrefs.GetFloat("sfxVolume", 1));
        }

        public void SetBGMVolume(float value)
        {
            if (value > 1)
                value /= 100;
            SetVolume(this.audioMixers.bgmMixer, value);
            PlayerPrefs.SetFloat("bgmVolume", value);
        }

        public void SetBGSVolume(float value)
        {
            SetVolume(this.audioMixers.bgsMixer, value);
        }

        public void SetSFXVolume(float value)
        {
            if (value > 1)
                value /= 100;
            SetVolume(this.audioMixers.sfxMixer, value);
            PlayerPrefs.SetFloat("sfxVolume", value);
        }

        private void SetVolume(AudioMixer mixer, float value)
        {
            mixer.SetFloat("MasterVolume", -80 * (1 - value));
        }

        #region play methods
        public void PlayBGS(AudioClip bgs)
        {
            Play(BGS.A, BGS.B, bgs);
        }

        public void PlayBGM(AudioClip bgm)
        {
            Play(BGM.A, BGM.B, bgm);
        }
        
        #endregion

        #region audio transitions
        public void CrossFade(AudioSource sourceA, AudioSource sourceB, AudioClip newClip, float newClipVolume)
        {
            var activeAudioSource = sourceA.isPlaying ? sourceA : sourceB;
            var freeAudioSource = sourceA.isPlaying ? sourceB : sourceA;

            freeAudioSource.clip = newClip;
            freeAudioSource.volume = 0;

            FadeOut(activeAudioSource);
            FadeIn(freeAudioSource, newClipVolume);
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
        #endregion

        private void StopTransition(AudioSource source)
        {
            Coroutine transition;
            if (this.transitions.TryGetValue(source, out transition))
            {
                if (transition != null)
                {
                    this.StopCoroutine(transition);
                    transition = null;
                }
                this.transitions.Remove(source);
            }
        }

        public AudioSource GetFreeSFXSource()
        {
            this.sfxSourcesPool.RemoveAll(source => !source);
            var freeAudioSource = this.sfxSourcesPool.Find(source => !source.isPlaying);
            if (!freeAudioSource)
            {
                if (this.sfxSourcesPool.Count < this.maxSFXSources)
                {
                    freeAudioSource = Instantiate(this.SFXSourcePrefab).GetComponent<AudioSource>();
                    this.sfxSourcesPool.Add(freeAudioSource);
                }
            }

            if (!freeAudioSource)
            {
                freeAudioSource = this.sfxSourcesPool[0];
                freeAudioSource.Stop();
            }

            freeAudioSource.outputAudioMixerGroup = this.audioMixers.sfxMixer.outputAudioMixerGroup;
            return freeAudioSource;
        }
        
        private void Play(AudioSource sourceA, AudioSource sourceB, AudioClip clip)
        {
            if (sourceA.isPlaying || sourceB.isPlaying)
            {
                CrossFade(sourceA, sourceB, clip, 1);
            }
            else
            {
                sourceA.clip = clip;
                FadeIn(sourceA, 1);
            }
        }

        #region transitions enumerators
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
        #endregion
    }
}
