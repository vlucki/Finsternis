namespace Finsternis
{
    using UnityEngine;
    using UnityQuery;

    public class OpenAction : EntityAction
    {

        [SerializeField]
        [Range(0.1f, 5f)]
        private float range = 0.3f;
        public KeyCard KeyCard { get; set; }

        public void Execute()
        {
            int mask = 1 << LayerMask.NameToLayer("Props");
            var hits = Physics.SphereCastAll(new Ray(transform.position + transform.up / 2, transform.forward), 0.5f, range, mask);
            if(hits != null){
                OpeneableEntity closestOpeneable = null;
                foreach(var hit in hits)
                {
                    var openeable = hit.collider.GetComponentInParentsOrChildren<OpeneableEntity>();
                    if (openeable)
                    {
                        if (!closestOpeneable ||
                            closestOpeneable.transform.position.Distance(transform.position) > openeable.transform.position.Distance(transform.position))
                        {
                                closestOpeneable = openeable;
                        }
                    }
                        
                }
                if(closestOpeneable)
                    closestOpeneable.Interact(this);
            }
        }
    }
}