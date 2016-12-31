namespace Finsternis
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Extensions;

    public class PhysicalAttack : Skill
    {
        [SerializeField]
        private GameObject[] hitboxes;

        private List<Collider> hitboxColliders;
        private List<TouchDamageHandler> damageHandlers;

        protected override void Awake()
        {
            base.Awake();

            hitboxColliders = new List<Collider>();
            damageHandlers = new List<TouchDamageHandler>();

            foreach (var hitBox in hitboxes)
            {
                var colliders = hitBox.GetComponents<Collider>();
                if (!colliders.IsNullOrEmpty())
                {
                    hitboxColliders.AddRange(colliders);
                }

                var handlers = hitBox.GetComponents<TouchDamageHandler>();
                if (!handlers.IsNullOrEmpty())
                {
                    damageHandlers.AddRange(handlers);
                }
            }

            ToggleHitbox(false);
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
            this.hitboxColliders.ForEach(col => col.enabled = enabled);
            this.damageHandlers.ForEach(handler => handler.enabled = enabled);
        }
    }
}