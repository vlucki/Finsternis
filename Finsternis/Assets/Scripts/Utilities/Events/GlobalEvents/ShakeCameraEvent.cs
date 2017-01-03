namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;

    public class ShakeCameraEvent : GlobalEventTrigger
    {
        [SerializeField]
        [Range(.1f, 10f)]
        private float shakeDuration = 1f;

        [SerializeField]
        [Range(1, 100)]
        private float shakeDamping = 2;

        [SerializeField]
        [Range(1, 100)]
        private float shakeAmplitude = 20;

        [SerializeField]
        [Range(1, 20)]
        private float shakeFrequency = 20;

        [SerializeField]
        private bool compoundShaking = false;

        public override void TriggerEvent()
        {
            if (GameManager.Instance && this.isActiveAndEnabled)
            {
                GameManager.Instance.TriggerGlobalEvent(
                    "ShakeCamera", 
                    transform.position,
                    compoundShaking,
                    shakeDuration,
                    shakeDamping,
                    shakeAmplitude,
                    shakeFrequency);
            }
        }
    }
}