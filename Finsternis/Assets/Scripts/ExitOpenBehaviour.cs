using UnityEngine;
using System.Collections;

public class ExitOpenBehaviour : StateMachineBehaviour {

    Transform originalCameraTarget;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Follow cameraFollow = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Follow>();
        originalCameraTarget = cameraFollow.target;
        cameraFollow.target = animator.gameObject.transform;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= 1)
        {
            Follow cameraFollow = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Follow>();
            cameraFollow.target = originalCameraTarget;
        }
    }
}
