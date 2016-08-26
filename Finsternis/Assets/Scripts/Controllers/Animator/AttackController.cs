using UnityEngine;
using System.Collections;

namespace Finsternis
{
    public class AttackController : StateMachineBehaviour
    {

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetInteger(CharController.AttackSlot, 0);
        }
    }
}