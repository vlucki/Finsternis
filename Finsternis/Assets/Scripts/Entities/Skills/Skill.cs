using MovementEffects;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Finsternis
{
    [RequireComponent(typeof(CharController))]
    public abstract class Skill : MonoBehaviour
    {
        [Header("General Skill attributes")]

        [SerializeField]
        [Range(0, 100)]
        protected float castTime = 0.2f;

        [SerializeField]
        [Range(0, 5)]
        protected float cooldownTime = 0.5f;

        [SerializeField]
        protected bool lockDuringCast = true;

        public UnityEvent onUse;
        public UnityEvent onCoolDownEnd;

        protected float lastUsed = 0;

        protected CharController user;

        private float timeDisabled;
        private bool equipped;

        private IEnumerator<float> castingHandle;
        private IEnumerator<float> cooldownHandle;

        public bool CoolingDown { get; private set; }
        public bool LockDuringCast { get { return lockDuringCast; } }
        public bool Equipped { get { return this.equipped; } }

        protected virtual void Awake()
        {
            user = GetComponent<CharController>();
        }

        public virtual void Use()
        {
            lastUsed = Time.timeSinceLevelLoad;
            this.castingHandle = Timing.RunCoroutine(_BeginCasting());
            if (onUse != null)
                onUse.Invoke();
        }

        public virtual bool MayUse()
        {
            return !CoolingDown;
        }

        private IEnumerator<float> _BeginCasting()
        {

            if (castTime > 0)
            {
                if (lockDuringCast)
                    user.Lock(castTime);
                yield return Timing.WaitForSeconds(castTime);
            }

            CastSkill();
        }

        protected virtual void CastSkill()
        {
            if (cooldownTime > 0)
                this.cooldownHandle = Timing.RunCoroutine(_Cooldown());
        }

        private IEnumerator<float> _Cooldown()
        {
            CoolingDown = true;

            yield return cooldownTime;

            CoolingDown = false;

            if (onCoolDownEnd != null)
                onCoolDownEnd.Invoke();
        }

        public virtual void Equip()
        {
            this.equipped = enabled = true;
        }

        public virtual void Unequip()
        {
            this.equipped = enabled = false;
        }

        protected virtual void OnDisable()
        {
            Timing.KillCoroutines(this.castingHandle);
            Timing.KillCoroutines(this.cooldownHandle);
            timeDisabled = Time.timeSinceLevelLoad;
        }

        protected virtual void OnEnable()
        {
            lastUsed -= (Time.timeSinceLevelLoad - timeDisabled);
            if (CoolingDown)
                Timing.RunCoroutine(_Cooldown());
        }
    }
}