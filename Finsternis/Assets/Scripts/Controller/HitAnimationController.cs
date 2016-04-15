using UnityEngine;
using System.Collections;

public class HitAnimationController : StateMachineBehaviour {
    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("hit", false);
    }
}
