using System;
using UnityEngine;

namespace Finsternis
{
    [RequireComponent(typeof(Character))]
    public class HealthAlert : AudioPlayer
    {
        [SerializeField]
        private EntityAttribute health;

        [SerializeField, Range(0, 1)]
        private float healthPercentageThreshold = .5f;

        [SerializeField]
        private AudioClip lowHealthAlert;

        private Character character;

        private bool alertPlaying = false;

        protected override void Awake()
        {
            base.Awake();
            this.character = GetComponent<Character>();
            this.character.onAttributeInitialized.AddListener(GrabHealth);
        }

        private void GrabHealth(EntityAttribute attribute)
        {
            if (attribute.Alias.Equals(health.Alias))
            {
                this.health = attribute;
                this.health.onValueChanged.AddListener(HealthChanged);
            }
        }

        private void HealthChanged(EntityAttribute attribute)
        {

            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().fieldOfView = 60 + (10 * (1 - attribute.Value / attribute.Max));
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