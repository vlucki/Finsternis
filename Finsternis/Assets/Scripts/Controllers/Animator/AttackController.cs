using UnityEngine;
using System.Collections;

namespace Finsternis
{
    public class AttackController : StateMachineBehaviour
    {

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(CharController.AttackBool, false);
            animator.SetInteger(CharController.AttackType, 0);
        }
    }
}