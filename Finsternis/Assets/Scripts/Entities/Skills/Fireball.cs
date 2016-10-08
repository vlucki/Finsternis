using UnityEngine;
using System.Collections;
using UnityQuery;

namespace Finsternis
{
    public class Fireball : Skill
    {
        [Space(10, order = 0)]
        [Header("Fireball attributes", order = 1)]

        [SerializeField]
        private GameObject fireballPrefab;

        [SerializeField]
        private Transform summonPoint;

        [SerializeField]
        [Tooltip("How much in front of the summon point the fireball will appear.")]
        [Range(-1, 1)]
        private float summonOffset = 0;

        private Animator animator;

        protected override void Awake()
        {
            base.Awake();
            if(!this.summonPoint)
                this.summonPoint = transform;
            this.animator = GetComponent<Animator>();
        }

        public override void Use()
        {
            animator.SetFloat(CharController.AttackSpeed, 5f);
            base.Use();
        }

        protected override void CastSkill()
        {
            base.CastSkill();

            GameObject summonedFireball = Instantiate(this.fireballPrefab, this.summonPoint.position + transform.forward * this.summonOffset, transform.rotation) as GameObject;

            var damageHandler = summonedFireball.GetComponent<TouchDamageHandler>();
            damageHandler.Ignore(gameObject);
            damageHandler.owner = user.Character;

            summonedFireball.SetActive(true);
            
            StartCoroutine(_Shoot(summonedFireball));
        }

        private IEnumerator _Shoot(GameObject summonedFireball)
        {
            yield return new WaitForFixedUpdate();
            summonedFireball.GetComponent<Rigidbody>().AddForce(summonedFireball.transform.forward * 75, ForceMode.Impulse);
            summonedFireball.GetComponent<AnimationEventInvoker>().onInvoked.AddListener(summonedFireball.DestroyNow);
        }
    }
}