namespace Finsternis
{
    using UnityEngine;
    
    using System.Collections;

    [RequireComponent(typeof(Renderer), typeof(AudioSource))]
    public class VideoTransition : Transition
    {
        private AudioSource audioIntro;
        private MovieTexture movie;

        protected override void Awake()
        {

            Renderer VideoIntro = GetComponent<Renderer>();
            movie = (MovieTexture)VideoIntro.material.mainTexture;
            audioIntro = GetComponent<AudioSource>();
            audioIntro.clip = movie.audioClip;

            if (OnTransitionStarted == null)
                OnTransitionStarted = new TransitionEvent();

            OnTransitionStarted.AddListener(t => StartCoroutine(_PlayVideo()));
            OnTransitionEnded.AddListener(t =>
            {
                if (movie.isPlaying)
                {
                    StopAllCoroutines();
                    movie.Stop();
                    audioIntro.Stop();
                }
            });
            base.Awake();
        }

        private IEnumerator _PlayVideo()
        {

            if (!movie.isPlaying)
            {
                audioIntro.Play();
                movie.Play();
            }
            while (movie.isPlaying)
                yield return null;
        }
    }
}