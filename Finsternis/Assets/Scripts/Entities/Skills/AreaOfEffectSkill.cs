namespace Finsternis
{
    using UnityEngine;
    using Extensions;

    public class AreaOfEffectSkill : Skill
    {
        [SerializeField, Range(.1f, 10f)]
        private float range = 5f;

        [SerializeField]
        private string tagToConsider = "Enemy";

        [SerializeField]
        private LayerMask layersOfEffect;

        [SerializeField]
        private EntityAttribute damageAttribute;

        [SerializeField]
        AttackData.DamageType damageType = AttackData.DamageType.physical;

        protected override void Awake()
        {
            base.Awake();
            this.GetComponent<Character>().onAttributeInitialized.AddListener(
                attribute => {
                if (attribute.Alias.Equals(this.damageAttribute.Alias))
                    this.damageAttribute = attribute;
            });
        }

        public override void StartExecution()
        {
            base.StartExecution();
            Collider[] inRange = Physics.OverlapSphere(transform.position, range, layersOfEffect);
            float damage = this.damageAttribute.Value * 1.1f;
            float explosionRange = this.range * 3f;
            foreach (var col in inRange)
            {
                if (col.CompareTag(tagToConsider))
                {
                    var atk = col.GetComponent<InteractionModule>();
                    if (atk)
                        atk.Interact(new AttackData(damage, GetComponent<Entity>(), this.damageType));
                    var body = col.GetComponentInParentsOrChildren<Rigidbody>();
                    if (body)
                    {
                        body.AddExplosionForce(1000, transform.position, explosionRange);
                    }
                }
            }
        }
    }
}