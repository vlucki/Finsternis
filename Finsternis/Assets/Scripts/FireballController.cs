namespace Finsternis
{
    using UnityEngine;
    using UnityQuery;

    public class FireballController : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            if (stateInfo.IsName("summon"))
            {
                animator.GetComponent<FireballEntity>().Shoot();
            }
            else if (stateInfo.IsName("explosion"))
            {
                animator.gameObject.Deactivate();
            }

        }
    }
}