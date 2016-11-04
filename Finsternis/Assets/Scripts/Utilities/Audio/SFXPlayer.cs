namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;

    public class SFXPlayer : AudioPlayer
    {
        public enum EffectType
        {
            GLOBAL = 0,
            WORLD = 1,
            LOCAL = 2
        }
        [SerializeField][Tooltip("GLOBAL = listen everywhere, WORLD = play at transform.position, LOCAL = play at transform.position and follow object around")]
        private EffectType type = EffectType.GLOBAL;

        public override void Play(AudioClip clip)
        {
            var source = this.Manager.PlayEffect(clip);
            if(this.type != EffectType.GLOBAL)
            {
                source.transform.position = this.transform.position;
                source.spatialBlend = 1;
                if (this.type == EffectType.LOCAL)
                    source.transform.SetParent(this.transform);
            }
        }
    }
}