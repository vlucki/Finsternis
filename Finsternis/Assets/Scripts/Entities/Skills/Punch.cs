namespace Finsternis
{
    using UnityEngine;
    using System.Collections;

    public class Punch : Skill
    {
        private Animator animator;

        protected override void Awake()
        {
            base.Awake();
            if (!this.onSkillCast)
                this.onSkillCast = new SkillEvent();
            this.animator = GetComponent<Animator>();
            this.onSkillCast.AddListener(skill => {
                    this.animator.SetTrigger(AttackController.EndAttackAnimationTrigger);
                });
        }
    }
}