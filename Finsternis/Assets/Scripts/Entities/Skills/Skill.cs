namespace Finsternis
{
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
        protected float startUpTime = 1f;
        
        [SerializeField]
        [Range(0, 100)]
        protected float castTime = 1f;

        [SerializeField]
        [Range(0, 100)]
        protected float endTime = 1f;

        [SerializeField]
        [Range(0, 5)]
        protected float cooldownTime = 0.5f;

        [Header("Skill phase events")]
        public SkillEvent onBegin;
        public SkillEvent onExecutionStart;
        public SkillEvent onExecutionEnd;
        public SkillEvent onEnd;
        public SkillEvent onCoolDownEnd;
        [Space(20)]

        protected float lastUsed = 0;

        protected CharController user;

        private float timeDisabled;

        public bool Using { get; private set; }
        public bool Casting { get; private set; }
        public bool Executing { get; private set; }
        public bool CoolingDown { get; private set; }
        
        [SerializeField]
        private AnimatorOverrideController skillOverrideController;
        private RuntimeAnimatorController defaultOverrideController;

        protected virtual void Awake()
        {
            user = GetComponent<CharController>();
            defaultOverrideController = user.Controller.runtimeAnimatorController;
        }

        public virtual void Begin()
        {
            Using = true;
            Casting = true;
            if (this.skillOverrideController)
                user.Controller.runtimeAnimatorController = this.skillOverrideController;

            user.Controller.SetTrigger(CharController.AttackTrigger);

            user.Controller.SetFloat(CharController.AttackSpeed, 1 / this.startUpTime);

            onBegin.Invoke(this);
        }

        public virtual void StartExecution()
        {
            Casting = false;
            Executing = true;

            user.Controller.SetFloat(CharController.AttackSpeed, 1 / this.castTime);

            onExecutionStart.Invoke(this);
        }

        public virtual void EndExecution()
        {
            Executing = false;

            user.Controller.SetFloat(CharController.AttackSpeed, 1 / this.endTime);

            onExecutionEnd.Invoke(this);
        }

        public virtual void End()
        {
            Using = false;
            user.Controller.SetFloat(CharController.AttackSpeed, 1);

            if (cooldownTime > 0)
                StartCoroutine(_Cooldown());

            if(this.skillOverrideController)
                user.Controller.runtimeAnimatorController = this.defaultOverrideController;
            
            onEnd.Invoke(this);
        }

        public bool MayUse()
        {
            return !Using;
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
            this.timeDisabled = Time.timeSinceLevelLoad;
        }

        protected virtual void OnEnable()
        {
            lastUsed -= (Time.timeSinceLevelLoad - this.timeDisabled);
            if (CoolingDown)
                StartCoroutine(_Cooldown());
        }
    }
}