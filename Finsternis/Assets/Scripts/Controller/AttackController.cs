using UnityEngine;
using System.Collections;

public class AttackController : StateMachineBehaviour {
    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("attacking", false);
    }
}
