using UnityEngine;
using UnityEngine.Experimental.Director;

namespace Finsternis
{
    public class ExitOpenController : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            base.OnStateExit(animator, stateInfo, layerIndex, controller);
            animator.GetComponent<Exit>().Open();
        }
    }
}