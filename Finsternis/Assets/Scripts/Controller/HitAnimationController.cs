using UnityEngine;
using System.Collections;

public class HitAnimationController : StateMachineBehaviour {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.SetBool(CharacterController.AttackBool, false);
    }
}
