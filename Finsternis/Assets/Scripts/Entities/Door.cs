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

        public override void Open()
        {
            base.Open();

            if (!IsOpen)
                return;

            int dir = (int)-transform.forward.z;

            if (LastInteraction)
            {
                Vector3 interactionSource = transform.position - LastInteraction.Agent.transform.position;

                dir = (transform.forward.Angle(interactionSource) < 90) ? -1 : 1;
            }
            Log.Info(this, "Animator = {0}", anim);
            anim.SetInteger("direction", dir);
            anim.SetTrigger("opening");
            var colliders = this.GetComponentsInParentsOrChildren<Collider>();
            colliders.ForEach(collider => collider.enabled = false);
            this.interactable = false;
        }

        public void ObjectInRange(GameObject obj)
        {
            var ctrl = obj.GetComponentInParentsOrChildren<CharController>();
            if (ctrl == GameManager.Instance.Player)
            {

                GameManager.Instance.ShowMessage(transform.position.WithY(2), "Press 'E' to open");
            }
        }
    }
}