namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;
    using Extensions;
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

        [SerializeField, Range(1, 30)]
        private float maxDistance = 10;

        [SerializeField, Range(1, 15)]
        private int maxSimultaneousClips = 7;

        [SerializeField, Tooltip("GLOBAL = listen everywhere, WORLD = play at transform.position, LOCAL = play at transform.position and follow object around")]
        private EffectType type = EffectType.GLOBAL;

        private List<AudioSource> activeSources;

        protected override void Awake()
        {
            base.Awake();
            this.activeSources = new List<AudioSource>(maxSimultaneousClips);
        }

        public override void Play(AudioClip clip)
        {
            if (!MayPlayAnotherClip())
                return;
            if (this.type != EffectType.GLOBAL && this.transform.position.Distance(GameManager.Instance.Player.transform.position) > this.maxDistance)
                return;

            GetManager();
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
            if (source.gameObject.activeInHierarchy)
            {
                source.Stop();
                source.Play();
            }
        }

        private bool MayPlayAnotherClip()
        {
            if (this.activeSources != null)
                this.activeSources.RemoveAll(source => !source || !source.gameObject.activeSelf);
            else
                this.activeSources = new List<AudioSource>();

            return this.activeSources.Count < this.maxSimultaneousClips;
        }
    }
}