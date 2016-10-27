namespace Finsternis
{
    using System;
    using UnityEngine;
    using UnityQuery;

    public class Punch : Skill
    {
        [SerializeField]
        private GameObject fist;

        private SphereCollider[] fistColliders;
        private TouchDamageHandler fistDamageHandler;

        protected override void Awake()
        {
            base.Awake();

            fistColliders = fist.GetComponents<SphereCollider>();
            fistDamageHandler = fist.GetComponent<TouchDamageHandler>();
        }

        public override void StartExecution()
        {
            base.StartExecution();
            TogglePunch(true);
        }

        public override void EndExecution()
        {
            base.EndExecution();
            TogglePunch(false);
        }

        private void TogglePunch(bool enabled)
        {
            foreach (var col in fistColliders)
                col.enabled = enabled;
            this.fistDamageHandler.enabled = enabled;
        }
    }
}