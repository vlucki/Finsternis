namespace Finsternis {
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using UnityQuery;

    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceExtension : MonoBehaviour
    {
        private AudioSource audioSource;

        [SerializeField]
        private List<AudioClip> clips;

        void Awake()
        {
            this.audioSource = GetComponent<AudioSource>();
        }

        public void PlayRandom()
        {
            this.audioSource.Stop();
            this.audioSource.PlayOneShot(clips.GetRandom(Random.Range));
        }
    }
}
