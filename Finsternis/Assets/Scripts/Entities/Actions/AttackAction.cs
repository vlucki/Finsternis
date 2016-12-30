using System;
using UnityEngine;
using UnityEngine.Events;
using UnityQuery;

namespace Finsternis
{
    [UnityEngine.DisallowMultipleComponent]
    public class AttackAction : EntityAction
    {
        [SerializeField]
        private EntityAttribute baseDamage;

        public UnityEvent onExecute;

        private DamageInfo dmgInfo;

        public DamageInfo DamageInfo
        {
            get { return dmgInfo; }
        }

        protected override void Awake()
        {
            base.Awake();

            if (this.baseDamage)
            {
                agent.onAttributeInitialized.AddListener(
                    attribute =>
                    {
                        if (attribute.Alias.Equals(baseDamage.Alias))
                        {
                            this.baseDamage = attribute;
                        }
                    }
                    );
            }
        }

        public void Execute(params IInteractable[] targets)
        {
            Execute(DamageInfo.DamageType.physical, 0, targets);
        }

        public void Execute(float extraDamage, params IInteractable[] targets)
        {
            Execute(DamageInfo.DamageType.physical, extraDamage, targets);
        }

        public void Execute(DamageInfo.DamageType damageType, params IInteractable[] targets)
        {
            Execute(damageType, 0, targets);
        }

        /// <summary>
        /// Performs an attack action.
        /// </summary>
        /// <param name="damageType">Type of damage that will be applied.</param>
        /// <param name="extraDamage">Value to be added to the base damage before applying it to the targets.</param>
        /// <param name="targets">One or more entities that will have damage applied to them.</param>
        public void Execute(DamageInfo.DamageType damageType, float extraDamage, params IInteractable[] targets)
        {
            if (targets == null || targets.Length < 1)
            {
#if DEBUG
                Log.E(this, "Cannot execute the attack logic without a target.");
#endif
                return;
            }

            float totalDamage = (this.baseDamage ? this.baseDamage.Value : 0) + extraDamage;

            this.dmgInfo = new DamageInfo(damageType, totalDamage, agent);

            foreach (Entity target in targets)
                target.Interact(this);

            this.onExecute.Invoke();
        }
    }
}