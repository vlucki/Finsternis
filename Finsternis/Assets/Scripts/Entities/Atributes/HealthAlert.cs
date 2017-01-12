using System;
using UnityEngine;

namespace Finsternis
{
    [RequireComponent(typeof(Character))]
    public class HealthAlert : AudioPlayer
    {
        [SerializeField]
        private EntityAttribute healthTemplate;

        [SerializeField, Range(0, 1)]
        private float healthPercentageThreshold = .5f;

        [SerializeField]
        private AudioClip lowHealthAlert;

        private Character character;

        private new Camera camera;

        private bool alertPlaying = false;

        protected override void Awake()
        {
            base.Awake();
            this.character = GetComponent<Character>();
            this.character.onAttributeInitialized.AddListener(GrabHealth);
            this.camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        private void GrabHealth(EntityAttribute attribute)
        {
            if (attribute.Alias.Equals(healthTemplate.Alias))
            {
                attribute.onValueChanged += HealthChanged;
            }
        }

        private void HealthChanged(EntityAttribute attribute)
        {

            this.camera.fieldOfView = 60 + (10 * (1 - attribute.Value / attribute.Max));
            if (attribute.Value <= attribute.Max * this.healthPercentageThreshold)
            {
                if (!alertPlaying)
                {
                    alertPlaying = true;
                    this.Play(lowHealthAlert);
                }
            }
            else if(alertPlaying)
            {
                alertPlaying = false;
                this.Manager.ReplayLastBGS();
            }

        }

        public override void Play(AudioClip clip)
        {
            this.Manager.PlayBGS(clip);
        }
    }
}