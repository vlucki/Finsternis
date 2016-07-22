using UnityEngine;
using System.Collections;
namespace Finsternis
{
    public class HitAnimationController : StateMachineBehaviour
    {

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            animator.SetBool(CharController.AttackTrigger, false);
            animator.GetComponent<CharController>().Lock();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<CharController>().Unlock();
        }
    }
}