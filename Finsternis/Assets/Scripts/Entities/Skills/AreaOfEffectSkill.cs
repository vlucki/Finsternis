namespace Finsternis
{
    using UnityEngine;
    using UnityQuery;

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
        DamageInfo.DamageType damageType = DamageInfo.DamageType.physical;

        protected override void Awake()
        {
            base.Awake();
            this.GetComponent<Character>().onAttributeInitialized.AddListener(attr => { if (attr.Alias.Equals(this.damageAttribute)) this.damageAttribute = attr; });
        }

        public override void StartExecution()
        {
            base.StartExecution();
            Collider[] inRange = Physics.OverlapSphere(transform.position, range, layersOfEffect);
            float extra = this.damageAttribute.Value * .1f;
            float explosionRange = this.range * 3f;
            foreach (var col in inRange)
            {
                if (col.CompareTag(tagToConsider))
                {
                    var enemy = col.GetComponentInParentsOrChildren<EnemyChar>();
                    if (enemy)
                        this.GetComponent<AttackAction>().Execute(this.damageType, extra, enemy);
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