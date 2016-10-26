namespace Finsternis
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityQuery;
    [RequireComponent(typeof(CharController))]
    public abstract class Skill : MonoBehaviour
    {
        [System.Serializable]
        public class SkillEvent : CustomEvent<Skill>
        {
        }

        [Header("General Skill attributes")]

        [SerializeField]
        [Range(0, 100)]
        protected float castTime = 0.2f;

        [SerializeField]
        [Range(0, 5)]
        protected float cooldownTime = 0.5f;

        [SerializeField]
        protected bool lockDuringCast = true;

        public SkillEvent onBeginCasting;
        public SkillEvent onEndCasting;
        public SkillEvent onCoolDownEnd;

        protected float lastUsed = 0;

        protected CharController user;

        private float timeDisabled;

        public bool Casting { get; private set; }
        public bool CoolingDown { get; private set; }
        public float CastTime { get { return this.castTime; } }


        protected virtual void Awake()
        {
            user = GetComponent<CharController>();
        }

        public virtual void CastSkill()
        {
            Casting = true;
            onBeginCasting.Invoke(this);
        }

        public virtual void End()
        {
            Casting = false;
            user.Animator.SetFloat(CharController.AttackSpeed, 1);
            onEndCasting.Invoke(this);
        }

        public bool MayUse()
        {
            return !Casting && !CoolingDown;
        }
        
        private IEnumerator _Cooldown()
        {
            CoolingDown = true;

            yield return Wait.Sec(cooldownTime);

            CoolingDown = false;

            if (onCoolDownEnd)
                onCoolDownEnd.Invoke(this);
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