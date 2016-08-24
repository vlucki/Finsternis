using System;
using UnityQuery;

namespace Finsternis
{

    [UnityEngine.DisallowMultipleComponent]
    public class AttackAction : EntityAction
    {
        EntityAttribute damage;
        private DamageInfo dmgInfo;

        public DamageInfo DamageInfo
        {
            get { return dmgInfo; }
        }

        protected override void Awake()
        {
            base.Awake();
            if (!damage)
                damage = agent.GetAttribute("dmg", true) as EntityAttribute;

            if (damage.Value == 0)
            {
                damage.SetValue(1);
            }
        }

        public void Execute(params Entity[] targets)
        {
            Execute(DamageInfo.DamageType.physical, 0, targets);
        }

        public void Execute(float extraDamage, params Entity[] targets)
        {
            Execute(DamageInfo.DamageType.physical, extraDamage, targets);
        }

        public void Execute(DamageInfo.DamageType damageType, params Entity[] targets)
        {
            Execute(damageType, 0, targets);
        }

        /// <summary>
        /// Performs an attack action.
        /// </summary>
        /// <param name="damageType">Type of damage that will be applied.</param>
        /// <param name="extraDamage">Value to be added to the base damage before applying it to the targets.</param>
        /// <param name="targets">One or more entities that will have damage applied to them.</param>
        public void Execute(DamageInfo.DamageType damageType, float extraDamage, params Entity[] targets)
        {
            if(targets == null || targets.Length < 1)
                throw new ArgumentException("Cannot execute the attack logic without a target.");

            float totalDamage = damage.Value + extraDamage;

            dmgInfo = new DamageInfo(damageType, totalDamage, agent);
            foreach (Entity target in targets)
                target.Interact(this);
        }
    }
}