namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;
    using Extensions;

    [RequireComponent(typeof(AudioSource))]
    public class SFXSourceController : MonoBehaviour
    {
        private AudioSource audioSource;

        void Awake()
        {
            this.audioSource = GetComponent<AudioSource>();
        }

        void OnEnable()
        {
            StartCoroutine(_WaitForAudioToPlay());
        }

        private IEnumerator _WaitForAudioToPlay()
        {
            yield return new WaitUntil(() => this.audioSource.isPlaying);
            if (!this.audioSource.loop)
                StartCoroutine(_DisableOnFinish());

        }

        private IEnumerator _DisableOnFinish()
        {
            yield return new WaitUntil(()=> !this.audioSource.isPlaying);

            this.gameObject.SetActive(false);
        }
    }
}