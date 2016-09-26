namespace Finsternis
{
    using UnityEngine;
    using UnityQuery;
    using System.Collections;

    [RequireComponent(typeof(Animator))]
    public class Door : Entity
    {
        private Animator anim;

        public static readonly int LockedBool = Animator.StringToHash("locked");
        public static readonly int OpenBool = Animator.StringToHash("open");

        protected override void Awake()
        {
            base.Awake();
            anim = GetComponent<Animator>();
        }

        public bool IsLocked()
        {
            return anim.GetBool(LockedBool);
        }

        public void Lock()
        {
            if (!anim.GetBool(OpenBool))
                anim.SetBool(LockedBool, true);
        }

        public void Unlock()
        {
            anim.SetBool(LockedBool, false);
        }

        public void Open()
        {
            if (IsLocked())
                return;
            int dir = (int)-transform.forward.z;

            if (lastInteraction)
            {
                Vector3 interactionSource = transform.position - lastInteraction.Agent.transform.position;

                dir = (transform.forward.Angle(interactionSource) < 90) ? -1 : 1;
            }
            anim.SetInteger("direction", dir);
            anim.SetTrigger("opening");
            StartCoroutine(_DisableCollider());
        }

        private IEnumerator _DisableCollider()
        {
            yield return Yields.SEC(1);
            GetComponentInChildren<Collider>().enabled = false;
        }
    }
}