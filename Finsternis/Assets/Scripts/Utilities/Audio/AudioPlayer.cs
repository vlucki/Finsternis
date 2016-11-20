namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.Audio;

    public abstract class AudioPlayer : CustomBehaviour
    {
        [SerializeField]
        protected AudioMixerGroup group;

        protected AudioManager Manager { get; private set; }

        public void SetGroup(AudioMixerGroup group) { this.group = group; }

        protected override void Awake()
        {
            base.Awake();

            var managerObj = GameObject.FindGameObjectWithTag("AudioManager");
            if (managerObj)
            {
                this.Manager = managerObj.GetComponent<AudioManager>();
            }

            if (!this.Manager)
            {
                this.Manager = FindObjectOfType<AudioManager>();
            }
        }

        public abstract void Play(AudioClip clip);
    }
}