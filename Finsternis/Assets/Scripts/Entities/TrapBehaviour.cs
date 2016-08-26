using UnityEngine;
using System.Collections.Generic;
using MovementEffects;
using System;

namespace Finsternis
{
    [RequireComponent(typeof(AttackAction))]
    public class TrapBehaviour : Entity
    {
        public float damageModifierOnTouch = 2;
        public float damageModifierOnStay = 1;

        public HashSet<Entity> entitiesInContact;

        protected Dungeon dungeon;

        protected Vector2 coordinates = -Vector2.one;

        protected AttackAction attack;

        protected override void Awake()
        {
            base.Awake();
            entitiesInContact = new HashSet<Entity>();
            if (!attack)
                attack = GetComponent<AttackAction>();
        }

        public virtual IEnumerator<float> _OnContinuousTouch(Entity e)
        {
            yield return 0f;
            while (entitiesInContact.Contains(e))
            {
                attack.Execute(DamageInfo.DamageType.physical, damageModifierOnStay, e);
                yield return 0f;
            }
        }

        public virtual void OnTouch(GameObject touched)
        {
            Entity e = touched.GetComponentInParent<Entity>();
            if (e)
            {
                if (!entitiesInContact.Contains(e))
                {
                    attack.Execute(DamageInfo.DamageType.physical, damageModifierOnTouch, e);
                    entitiesInContact.Add(e);
                    Timing.RunCoroutine(_OnContinuousTouch(e));
                }
            }
        }

        public virtual void OnExit(GameObject exited)
        {
            Entity e = exited.GetComponentInParent<Entity>();
            if (e)
            {
                entitiesInContact.Remove(e);
            }
        }
    }
}