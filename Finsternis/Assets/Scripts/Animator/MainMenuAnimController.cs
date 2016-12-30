using UnityEngine;
using System.Collections;

namespace Finsternis
{
    public class MainMenuAnimController : StateMachineBehaviour
    {

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            animator.SetBool("FadedIn", true);
        }
    }
}