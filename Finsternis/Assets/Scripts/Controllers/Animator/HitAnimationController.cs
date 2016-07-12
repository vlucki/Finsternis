using UnityEngine;
using System.Collections;
namespace Finsternis
{
    public class HitAnimationController : StateMachineBehaviour
    {

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            animator.SetBool(CharacterController.AttackBool, false);
            animator.GetComponent<CharacterController>().Lock();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<CharacterController>().Unlock();
        }
    }
}