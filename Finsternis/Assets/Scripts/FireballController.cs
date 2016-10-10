namespace Finsternis
{
    using UnityEngine;
    using UnityQuery;

    public class FireballController : StateMachineBehaviour
    {
        private FireballEntity fireball;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            if (!this.fireball)
                this.fireball = animator.GetComponent<FireballEntity>();

            if (this.fireball)
            {
                if (stateInfo.IsName("explosion"))
                {
                    animator.GetComponent<FireballEntity>().Explode();
                }
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);

            if (!this.fireball)
                this.fireball = animator.GetComponent<FireballEntity>();

            if (this.fireball)
            {
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
}