namespace Finsternis
{
    using UnityEngine;
    using MovementEffects;
    using System.Collections.Generic;

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

            OnTransitionStarted.AddListener(t => Timing.RunCoroutine(_PlayVideo()));
            OnTransitionEnded.AddListener(t =>
            {
                if (movie.isPlaying)
                {
                    Timing.KillCoroutines(_PlayVideo());
                    movie.Stop();
                    audioIntro.Stop();
                }
            });
            base.Awake();
        }

        private IEnumerator<float> _PlayVideo()
        {

            if (!movie.isPlaying)
            {
                audioIntro.Play();
                movie.Play();
            }
            while (movie.isPlaying)
                yield return 0;
        }
    }
}