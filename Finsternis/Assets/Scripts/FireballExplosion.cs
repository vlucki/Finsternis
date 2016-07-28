using UnityEngine;

namespace Finsternis
{
    public class FireballExplosion : StateMachineBehaviour
    {

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            float dist = Vector3.Distance(
                new Vector3(player.transform.position.x, animator.transform.position.y, player.transform.position.z),
                animator.transform.position);
            if (dist <= 0)
                dist = 1;
            cam.GetComponent<CameraController>().Shake(0.75f, 4, 20 / dist, 20);
        }
    }
}