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

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.IsTag("Execution"))
            {
                Debug.Log("Executing skill animator");
                var controller = animator.GetComponent<CharController>();
                controller.ActiveSkill.StartExecution();
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.IsTag("Execution"))
            {
                var controller = animator.GetComponent<CharController>();
                controller.ActiveSkill.EndExecution();
            }
            else if (stateInfo.IsTag("End"))
            {
                animator.ResetTrigger(CharController.AttackTrigger);
                var controller = animator.GetComponent<CharController>();
                controller.ActiveSkill.End();
            }
        }
    }
}