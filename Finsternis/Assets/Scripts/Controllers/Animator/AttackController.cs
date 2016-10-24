namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.Events;

    public class AttackController : StateMachineBehaviour
    {
        public static readonly int EndAttackAnimationTrigger;

        static AttackController()
        {
            EndAttackAnimationTrigger = Animator.StringToHash("endAttack");
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.IsTag("AtkEnd"))
            {
                animator.SetInteger(CharController.AttackSlot, -1);
                var controller = animator.GetComponent<CharController>();
                foreach (var skill in controller.EquippedSkills)
                    if(skill)
                        skill.SkillFinished();
            }
        }
    }
}