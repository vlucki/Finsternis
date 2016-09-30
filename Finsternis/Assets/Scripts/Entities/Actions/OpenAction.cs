namespace Finsternis
{
    using UnityEngine;
    using UnityQuery;

    public class OpenAction : EntityAction
    {

        [SerializeField]
        [Range(0.1f, 1f)]
        private float range = 0.3f;
        public KeyCard KeyCard { get; set; }

        protected override void Awake()
        {
            base.Awake();
            Type = ActionType.OPEN;
        }

        public void Execute()
        {
            RaycastHit hit;
            if(Physics.SphereCast(new Ray(transform.position, transform.forward), range, out hit))
            {
                var openeable = hit.collider.GetComponentInSibling<OpeneableEntity>();

                if (openeable)
                    openeable.Interact(this);
            }
        }
    }
}