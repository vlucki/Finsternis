using UnityEngine;
using System.Collections.Generic;
using MovementEffects;

namespace Finsternis
{
    public class Fireball : Skill
    {
        [Header("Fireball attributes")]

        [SerializeField]
        private GameObject _fireballPrefab;

        [SerializeField]
        private Transform _summonPoint;

        [SerializeField]
        [Tooltip("How much in front of the summon point the fireball will appear.")]
        [Range(-1, 1)]
        private float _summonOffset = 0;

        protected override void Use(int _slot)
        {
            if (MayUse(_slot))
            {
                base.Use(_slot);
                GetComponent<Animator>().SetFloat("attackSpeed", 5f);
            }
        }

        protected override void CastSkill()
        {
            GameObject summonedFireball = Instantiate(_fireballPrefab, _summonPoint.position + transform.forward * _summonOffset, transform.rotation) as GameObject;
            PhysicalAttackHandler pah = summonedFireball.GetComponent<PhysicalAttackHandler>();
            pah.ignoreColliders.Add(GetComponent<Collider>());
            pah.owner = GetComponent<Entity>();
            summonedFireball.SetActive(true);
            Timing.RunCoroutine(_Shoot(summonedFireball), Segment.FixedUpdate);
        }

        private IEnumerator<float> _Shoot(GameObject summonedFireball)
        {
            summonedFireball.GetComponent<Rigidbody>().AddForce(summonedFireball.transform.forward * 50, ForceMode.Impulse);
            yield return 0f;
        }
    }
}