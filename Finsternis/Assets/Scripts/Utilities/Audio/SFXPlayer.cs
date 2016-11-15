namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;
    using UnityQuery;
    using System.Collections.Generic;
    using UnityEngine.Audio;

    public class SFXPlayer : AudioPlayer
    {
        public enum EffectType
        {
            GLOBAL = 0,
            WORLD = 1,
            LOCAL = 2
        }

        [SerializeField][Range(1, 20)]
        private int maxSimultaneousClips = 10;

        [SerializeField][Tooltip("GLOBAL = listen everywhere, WORLD = play at transform.position, LOCAL = play at transform.position and follow object around")]
        private EffectType type = EffectType.GLOBAL;

        private List<AudioSource> activeSources;

        protected override void Awake()
        {
            base.Awake();
            this.activeSources = new List<AudioSource>(10);
        }

        public override void Play(AudioClip clip)
        {
            if (!MayPlayAnotherClip())
                return;

            var source = this.Manager.GetFreeSFXSource();
            this.activeSources.Add(source);
            if (this.group)
                source.outputAudioMixerGroup = this.group;

            var follow = source.GetComponent<Follow>();
            if (this.type != EffectType.GLOBAL)
            {
                source.transform.SetParent(this.transform);
                source.transform.localPosition = Vector3.zero;
                source.transform.SetParent(null);
                source.spatialBlend = 1;
                if (this.type == EffectType.LOCAL)
                {
                    follow.SetTarget(this.transform);
                    follow.Enable();
                }
            }
            else
            {
                follow.Disable();
                source.spatialBlend = 0;
            }
            source.clip = clip;
            source.gameObject.SetActive(true);
            source.Stop();
            source.Play();
        }

        private bool MayPlayAnotherClip()
        {
            this.activeSources.RemoveAll(source => !source || !source.gameObject.activeSelf);

            return this.activeSources.Count < this.maxSimultaneousClips;
        }
    }
}