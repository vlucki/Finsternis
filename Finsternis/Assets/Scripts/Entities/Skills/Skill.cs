using MovementEffects;
using System.Collections.Generic;
using UnityEngine;

namespace Finsternis
{
    [RequireComponent(typeof(CharacterController))]
    public abstract class Skill : MonoBehaviour
    {
        [Header("General Skill attributes")]
        [Tooltip("Which of the skill slots is occupied by this particular skill?")]
        [SerializeField]
        private int _slot;

        [SerializeField]
        [Range(0, 5)]
        protected float castTime = 0.2f;

        [SerializeField]
        [Range(0, 5)]
        protected float cooldownTime = 0.5f;

        protected float remainingCooldown = 0;

        private IEnumerator<float> _castingHandle;
        private IEnumerator<float> _cooldownHandle;

        public bool CollingDown { get { return remainingCooldown > 0; } }

        protected virtual void Awake()
        {
            GetComponent<CharacterController>().onAttack.AddListener(Use);
        }

        protected virtual void Use(int _slot)
        {
            if (MayUse(_slot))
            {
                remainingCooldown = cooldownTime;
                _castingHandle = Timing.RunCoroutine(_BeginCasting());
                if (CollingDown)
                    _cooldownHandle = Timing.RunCoroutine(_Cooldown());
            }
        }

        public bool MayUse(int _slot)
        {
            return !CollingDown && _slot == this._slot;
        }

        private IEnumerator<float> _BeginCasting()
        {
            if (castTime > 0)
                yield return Timing.WaitForSeconds(castTime);
            CastSkill();
        }

        protected abstract void CastSkill();

        private IEnumerator<float> _Cooldown()
        {
            do
            {
                yield return 0f;
                remainingCooldown -= Time.deltaTime;
            } while (CollingDown);
        }

        void OnDisable()
        {
            Timing.KillCoroutine(_castingHandle);
            Timing.KillCoroutine(_cooldownHandle);
        }

        void OnDestroy()
        {
            Timing.KillCoroutine(_castingHandle);
            Timing.KillCoroutine(_cooldownHandle);
        }
    }
}