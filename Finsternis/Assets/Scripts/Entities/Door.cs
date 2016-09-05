namespace Finsternis
{
    using UnityEngine;
    using UnityQuery;
    using System.Collections;

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
            StartCoroutine(_DisableCollider());
        }

        private IEnumerator _DisableCollider()
        {
            yield return Yields.Seconds(1);
            GetComponent<Collider>().enabled = false;
        }
    }
}