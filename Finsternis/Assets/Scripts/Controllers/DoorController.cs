using MovementEffects;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DoorController : MonoBehaviour
{

    public void Open()
    {
        Animator anim = GetComponent<Animator>();
        int dir = (int)-transform.forward.z;

        if (GetComponent<Entity>().lastInteraction)
        {
            Vector3 damageSource = transform.position - GetComponent<Entity>().lastInteraction.Agent.transform.position;
            dir = (Vector3.Angle(transform.forward, damageSource) < 90) ? -1 : 1;
        }
        anim.SetInteger("direction", dir);
        anim.SetTrigger("opening");
        Timing.RunCoroutine(_DisableCollider());
    }

    private IEnumerator<float> _DisableCollider()
    {
        yield return Timing.WaitForSeconds(1);
        GetComponent<Collider>().enabled = false;
    }
}
