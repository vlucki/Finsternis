namespace Finsternis
{
    using UnityEngine;
    using Extensions;
    using System.Collections;
    using System.Collections.Generic;
    using System;

    [RequireComponent(typeof(Animator))]
    public class Door : OpeneableEntity
    {
        private Animator anim;

        private MessageController displayedTooltip;

        protected override void Awake()
        {
            base.Awake();
            anim = GetComponent<Animator>();
            anim.SetBool(LockedBool, IsLocked);
        }

        public override void Open()
        {
            base.Open();

            if (!IsOpen)
                return;

            int dir = (int)-transform.forward.z;
            
            anim.SetInteger("direction", dir);
            anim.SetTrigger("opening");
            var colliders = this.GetComponentsInParentsOrChildren<Collider>();
            colliders.ForEach(collider => collider.enabled = false);
        }
    }
}