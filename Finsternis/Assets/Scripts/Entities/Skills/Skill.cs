using MovementEffects;
using System.Collections.Generic;
using UnityEngine;

namespace Finsternis
{
    [RequireComponent(typeof(CharController))]
    public abstract class Skill : MonoBehaviour
    {
        [Header("General Skill attributes")]

        [Tooltip("Which of the skill slots is occupied by this particular skill?")]
        [SerializeField]
        private int _slot;

        [SerializeField]
        [Range(0, 100)]
        protected float castTime = 0.2f;

        [SerializeField]
        [Range(0, 5)]
        protected float cooldownTime = 0.5f;

        [SerializeField]
        private bool _lockDuringCast = true;

        protected float remainingCooldown = 0;

        private IEnumerator<float> _castingHandle;
        private IEnumerator<float> _cooldownHandle;

        private CharController user;

        public bool CollingDown { get { return remainingCooldown > 0; } }
        public bool LockDuringCast
        {
            get { return _lockDuringCast; }
            set { _lockDuringCast = value; }
        }

        protected virtual void Awake()
        {
            user = GetComponent<CharController>();
            user.onAttack.AddListener(Use);
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
            if (LockDuringCast)
                user.Lock();

            if (castTime > 0)
                yield return Timing.WaitForSeconds(castTime);

            CastSkill();

            if (LockDuringCast)
                user.Unlock();
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