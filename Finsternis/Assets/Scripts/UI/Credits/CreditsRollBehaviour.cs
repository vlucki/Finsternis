namespace Finsternis {
    using UnityEngine;

    public class CreditsRollBehaviour : StateMachineBehaviour {

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponentInParent<CreditsController>().BeginClosing();
        }
    }
}