namespace Finsternis
{

    using System.Collections;
    using UnityEngine;
    using UnityQuery;
    [RequireComponent(typeof(CharController))]
    public abstract class Skill : MonoBehaviour
    {
        [System.Serializable]
        public class SkillEvent : CustomEvent<Skill> { }

        [Header("General Skill attributes")]

        [SerializeField]
        [Range(0, 100)]
        protected float castTime = 0.2f;

        [SerializeField]
        [Range(0, 5)]
        protected float cooldownTime = 0.5f;

        [SerializeField]
        protected bool lockDuringCast = true;

        public SkillEvent onBeginUse;
        public SkillEvent onSkillCast;
        public SkillEvent onCoolDownEnd;

        protected float lastUsed = 0;

        protected CharController user;

        private float timeDisabled;
        private bool equipped;

        public bool Casting { get; private set; }
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
            StartCoroutine(_BeginCasting());
            if (onBeginUse)
                onBeginUse.Invoke(this);
        }

        public virtual bool MayUse()
        {
            return !CoolingDown && !Casting;
        }

        private IEnumerator _BeginCasting()
        {
            Casting = true;
            if (castTime > 0)
            {
                if (lockDuringCast)
                    user.Lock(castTime);
                yield return Wait.Sec(castTime);
            }

            Casting = false;
            CastSkill();
        }

        protected virtual void CastSkill()
        {
            onSkillCast.Invoke(this);
            if (cooldownTime > 0)
                StartCoroutine(_Cooldown());
        }

        private IEnumerator _Cooldown()
        {
            CoolingDown = true;

            yield return Wait.Sec(cooldownTime);

            CoolingDown = false;

            if (onCoolDownEnd)
                onCoolDownEnd.Invoke(this);
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
            StopAllCoroutines();
            timeDisabled = Time.timeSinceLevelLoad;
        }

        protected virtual void OnEnable()
        {
            lastUsed -= (Time.timeSinceLevelLoad - timeDisabled);
            if (CoolingDown)
                StartCoroutine(_Cooldown());
        }
    }
}