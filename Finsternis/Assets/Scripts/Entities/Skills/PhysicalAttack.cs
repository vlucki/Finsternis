namespace Finsternis
{
    using System;
    using UnityEngine;
    using UnityQuery;

    public class PhysicalAttack : Skill
    {
        [SerializeField]
        private GameObject hitbox;

        private Collider[] hitboxColliders;
        private TouchDamageHandler damageHandler;

        protected override void Awake()
        {
            base.Awake();

            hitboxColliders = hitbox.GetComponents<Collider>();
            damageHandler = hitbox.GetComponent<TouchDamageHandler>();
        }

        public override void StartExecution()
        {
            base.StartExecution();
            ToggleHitbox(true);
        }

        public override void EndExecution()
        {
            base.EndExecution();
            ToggleHitbox(false);
        }

        private void ToggleHitbox(bool enabled)
        {
            foreach (var col in hitboxColliders)
                col.enabled = enabled;
            this.damageHandler.enabled = enabled;
        }
    }
}