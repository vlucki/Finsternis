using System;
using System.Collections;
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
        StartCoroutine(DisableCollider());
    }

    private IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(1);
        GetComponent<Collider>().enabled = false;
    }
}
