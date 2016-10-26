using UnityEngine;
using System.Collections;
using UnityQuery;
using System.Collections.Generic;
using System;

namespace Finsternis
{
    public class FireballSkill : Skill
    {
        [Space(10, order = 0)]
        [Header("Fireball attributes", order = 1)]

        [SerializeField]
        private GameObject fireballPrefab;

        [SerializeField]
        private Transform summonPoint;

        [SerializeField]
        [Tooltip("How much in front of the summon point the fireball will appear.")]
        [Range(-1, 1)]
        private float summonOffset = 0;

        private Animator animator;

        private List<GameObject> fireballs;

        protected override void Awake()
        {
            base.Awake();
            if (!this.summonPoint)
                this.summonPoint = transform;
            this.animator = GetComponent<Animator>();
        }

        public override void CastSkill()
        {
            GameObject summonedFireball = GetFireball();
            summonedFireball.transform.position = this.summonPoint.position + transform.forward * this.summonOffset;
            summonedFireball.transform.rotation = transform.rotation;

            var damageHandler = summonedFireball.GetComponent<TouchDamageHandler>();
            damageHandler.Ignore(gameObject);
            damageHandler.Owner = user.Character;

            var movement = summonedFireball.GetComponent<MovementAction>();
            movement.Direction = transform.forward;
            summonedFireball.GetComponent<FireballEntity>().OnShoot.AddListener(() => this.animator.SetTrigger(AttackController.EndAttackAnimationTrigger));
            summonedFireball.Activate();

            base.CastSkill();
        }

        private GameObject GetFireball()
        {
            if (fireballs == null || fireballs.Count == 0)
            {
                fireballs = new List<GameObject>(1);
            }

            GameObject fireball = null;
            for (int i = fireballs.Count - 1; i >= 0; i--)
            {
                var fireballOnPool = fireballs[i];
                if (!fireballOnPool)
                {
                    fireballs.Remove(fireballOnPool);
                }
                else if (!fireballOnPool.activeSelf)
                {
                    return fireballOnPool;
                }
            }

            if (!fireball)
            {
                fireball = Instantiate(this.fireballPrefab);
                fireballs.Add(fireball);
            }

            return fireball;

        }
    }
}