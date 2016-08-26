namespace Finsternis
{
    using MovementEffects;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using UnityQuery;

    [RequireComponent(typeof(Animator))]
    public class Door : Entity
    {
        private Animator anim;

        private static readonly int DoorLockedBool = Animator.StringToHash("locked");
        private static readonly int DoorOpenedBool = Animator.StringToHash("opened");

        protected override void Awake()
        {
            base.Awake();
            anim = GetComponent<Animator>();
        }

        public bool IsLocked()
        {
            return anim.GetBool(DoorLockedBool);
        }

        public void Lock()
        {
            if (!anim.GetBool(DoorOpenedBool))
                anim.SetBool(DoorLockedBool, true);
        }

        public void Unlock()
        {
            anim.SetBool(DoorLockedBool, false);
        }

        private void CheckInteraction()
        {
            Open();
        }

        public override void Interact(EntityAction action)
        {
            base.Interact(action);
            if(!IsLocked())
                Open();
        }

        public void Open()
        {
            int dir = (int)-transform.forward.z;

            if (lastInteraction)
            {
                Vector3 interactionSource = transform.position - lastInteraction.Agent.transform.position;
                
                dir = (transform.forward.GetAngle(interactionSource) < 90) ? -1 : 1;
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
}