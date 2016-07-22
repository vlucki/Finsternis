using MovementEffects;
using System.Collections.Generic;
using UnityEngine;

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

        protected float remainingCooldown = 0;

        protected CharController user;

        private float timeDisabled;
        private bool equipped;

        private IEnumerator<float> castingHandle;
        private IEnumerator<float> cooldownHandle;

        public bool CoolingDown { get { return remainingCooldown > 0; } }
        public bool LockDuringCast { get { return lockDuringCast; } }
        public bool Equipped { get { return this.equipped; } }

        protected virtual void Awake()
        {
            user = GetComponent<CharController>();
        }

        public virtual bool Use()
        {
            if (MayUse())
            {
                remainingCooldown = cooldownTime;
                this.castingHandle = Timing.RunCoroutine(_BeginCasting());
                return true;
            }
            return false;
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
            if (CoolingDown)
                this.cooldownHandle = Timing.RunCoroutine(_Cooldown());
        }

        private IEnumerator<float> _Cooldown()
        {
            do
            {
                yield return 0f;
                remainingCooldown -= Time.deltaTime;
            } while (CoolingDown);
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
            Timing.KillCoroutine(this.castingHandle);
            Timing.KillCoroutine(this.cooldownHandle);
            timeDisabled = Time.timeSinceLevelLoad;
        }

        protected virtual void OnEnable()
        {
            remainingCooldown -= (Time.timeSinceLevelLoad - timeDisabled);
            if(CoolingDown)
                Timing.RunCoroutine(_Cooldown());
        }
    }
}