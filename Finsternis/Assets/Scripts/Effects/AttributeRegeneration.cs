namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;
    using Extensions;

    public class AttributeRegeneration : Effect
    {
        public enum RegenType { ABSOLUTE = 0, RELATIVE = 1 }
        
        private RegenType regenType = RegenType.ABSOLUTE;
        
        private float regenAmount;
        
        private float regenInterval = 1;

        public AttributeRegeneration(Entity entity, Attribute attribute, RegenType type, float amount, float interval)
        {
            this.regenType = type;
            this.regenAmount = amount;
            this.regenInterval = interval;
            entity.StartCoroutine(_Regen(attribute));
        }

        private IEnumerator _Regen(Attribute attribute)
        {
            while (ShouldBeActive())
            {
                float amount = this.regenAmount;
                if (this.regenType == RegenType.RELATIVE)
                {
                    foreach(var constraint in attribute.Constraints)
                    {
                        if(constraint.Type == AttributeConstraint.AttributeConstraintType.MAX)
                        {
                            amount *= constraint.Value;
                            break;
                        }

                    }
                }

                attribute.Value += (amount);
                if (this.regenInterval > 0)
                    yield return WaitHelpers.Sec(this.regenInterval);
                else
                    yield return null;
            }
        }
    }
}