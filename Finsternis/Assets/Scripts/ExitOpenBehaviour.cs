using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Director;

public class ExitOpenBehaviour : StateMachineBehaviour {

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
    {
        base.OnStateExit(animator, stateInfo, layerIndex, controller);
        animator.GetComponent<Exit>().Open();
    }
}
