namespace Finsternis
{
    using System;
    using UnityEngine;
    using UnityQuery;

    public class Punch : Skill
    {
        private Animator animator;

        [SerializeField]
        private GameObject fist;

        [SerializeField][Range(0,1)]
        private float colliderActivationDelay = 0;

        private SphereCollider[] fistColliders;
        private TouchDamageHandler fistDamageHandler;

        protected override void Awake()
        {
            base.Awake();
            if (!this.onSkillCast)
                this.onSkillCast = new SkillEvent();
            this.animator = GetComponent<Animator>();
            this.onSkillCast.AddListener(skill => {
                    this.animator.SetTrigger(AttackController.EndAttackAnimationTrigger);
                this.CallDelayed(colliderActivationDelay, EnablePunch);
                });

            this.onSkillFinished.AddListener(skill =>
            {
                TogglePunch(false);
            });

            fistColliders = fist.GetComponents<SphereCollider>();
            fistDamageHandler = fist.GetComponent<TouchDamageHandler>();
        }

        private void TogglePunch(bool enabled)
        {
            foreach (var col in fistColliders)
                col.enabled = enabled;
            this.fistDamageHandler.enabled = enabled;
        }

        private void EnablePunch()
        {
            TogglePunch(true);
        }
    }
}