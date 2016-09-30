namespace Finsternis
{
    using UnityEngine;
    using UnityQuery;
    using System.Collections;
    using System.Collections.Generic;

    [RequireComponent(typeof(Animator))]
    public class Door : OpeneableEntity
    {
        private Animator anim;

        public static readonly int LockedBool = Animator.StringToHash("locked");
        public static readonly int OpenBool = Animator.StringToHash("open");
        

        protected override void Awake()
        {
            base.Awake();

            anim = GetComponent<Animator>();
            anim.SetBool(LockedBool, IsLocked);

        }

        public void ForceOpen()
        {
            if (!IsOpen)
                OpenDoor();
        }

        public void OpenDoor()
        {
            int dir = (int)-transform.forward.z;

            if (LastInteraction)
            {
                Vector3 interactionSource = transform.position - LastInteraction.Agent.transform.position;

                dir = (transform.forward.Angle(interactionSource) < 90) ? -1 : 1;
            }
            anim.SetInteger("direction", dir);
            anim.SetTrigger("opening");
            GetComponentInChildren<Collider>().enabled = false;
            this.interactable = false;
        }
    }
}