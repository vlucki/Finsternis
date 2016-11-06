namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.Events;
    using UnityEngine.Experimental.Director;

    public class AttackController : StateMachineBehaviour
    {

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.IsTag("Execution"))
            {
                Debug.Log("Starting skill execution");
                var controller = animator.GetComponent<CharController>();
                controller.ActiveSkill.StartExecution();
            }
            else if (stateInfo.IsTag("End"))
            {
                Debug.Log("Last skill phase");
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex, controller);
            Debug.Log(stateInfo);
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.IsTag("Execution"))
            {
                Debug.Log("Ending skill execution");
                var controller = animator.GetComponent<CharController>();
                controller.ActiveSkill.EndExecution();
            }
            else if (stateInfo.IsTag("End"))
            {
                Debug.Log("Ending skill");
                var controller = animator.GetComponent<CharController>();
                controller.ActiveSkill.End();
            }
        }
    }
}