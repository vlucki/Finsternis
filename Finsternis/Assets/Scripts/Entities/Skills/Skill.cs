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
        private new string name;

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

        private float timeStarted;

        public string Name { get { return this.name; } }
        public bool Using { get; private set; }
        public bool Casting { get; private set; }
        public bool Executing { get; private set; }
        public bool CoolingDown { get; private set; }

        protected virtual void Awake()
        {
            user = GetComponent<CharController>();
        }

        public virtual void Begin()
        {
            Using = true;
            Casting = true;
            this.timeStarted = Time.timeSinceLevelLoad;

            onBegin.Invoke(this);

            user.Controller.SetFloat(CharController.AttackSpeed, 1 / this.startUpTime);
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
            user.Controller.SetFloat(CharController.AttackSpeed, 1);

            if (cooldownTime > 0)
                StartCoroutine(_Cooldown());
            
            onEnd.Invoke(this);
            Using = false;
        }

        public bool MayUse()
        {
            if (Using)
                CheckElapsedTime();
            return !Using && !CoolingDown;
        }

        /// <summary>
        /// Sometimes the "end" method of a skill is not called. This ensures the skill is not treated as if it's still being used in such cases.
        /// </summary>
        private void CheckElapsedTime()
        {
            Using = (Time.timeSinceLevelLoad - this.timeStarted) > (this.startUpTime + this.castTime + this.endTime);
        }

        private IEnumerator _Cooldown()
        {
            CoolingDown = true;

            yield return Wait.Sec(cooldownTime);

            CoolingDown = false;

            if (onCoolDownEnd)
                onCoolDownEnd.Invoke(this);
        }
    }
}