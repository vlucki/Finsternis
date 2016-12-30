namespace Finsternis
{
    using UnityEngine;
    using UnityQuery;

    public class AttackController : StateMachineBehaviour
    {

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.IsTag("Execution"))
            {
#if LOG_INFO
                Log.I(this, "Starting skill execution");
#endif

                var controller = animator.GetComponent<CharController>();
                if (controller.ActiveSkill)
                    controller.ActiveSkill.StartExecution();

#if LOG_INFO || LOG_WARN
                else
                    Log.W(this, "Cannot start execution when skill is not active!");
#endif
            }
#if LOG_INFO
            else if (stateInfo.IsTag("End"))
            {
                Debug.Log("Last skill phase");

            }
#endif

        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.IsTag("Execution"))
            {
#if LOG_INFO
                Log.I(this, "Ending skill execution");
#endif

                var controller = animator.GetComponent<CharController>();
                if (controller.ActiveSkill)
                    controller.ActiveSkill.EndExecution();

#if LOG_INFO || LOG_WARN
                else
                    Log.W(this, "Cannot end execution when skill is not active!");
#endif
            }
            else if (stateInfo.IsTag("End"))
            {
#if LOG_INFO
                Log.I(this, "Ending skill");
#endif

                var controller = animator.GetComponent<CharController>();
                if (controller.ActiveSkill)
                    controller.ActiveSkill.End();

#if LOG_INFO || LOG_WARN
                else
                    Log.W(this, "Cannot end inactive skill!");
#endif

            }
        }
    }
}