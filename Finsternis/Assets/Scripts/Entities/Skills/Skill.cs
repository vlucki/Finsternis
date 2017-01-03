namespace Finsternis
{
    using EasyEditor;
    using System;
    using System.Collections;
    using System.Linq;
    using UnityEngine;
    using Extensions;
    [RequireComponent(typeof(CharController))]
    public abstract class Skill : MonoBehaviour
    {
        [System.Serializable]
        public class SkillEvent : Events.CustomEvent<Skill>
        {
        }

        [Inspector(group = "Basic attributes")]
        [SerializeField]
        private new string name;

        [SerializeField, Range(0, 100)]
        private float energyCost = 1;

        [SerializeField]
        private AttributeTemplate energyAttributeTemplate;

        [Inspector(group = "Phases duration", foldable = true)]
        [SerializeField, Range(0, 100)]
        protected float startUpTime = 1f;
        
        [SerializeField, Range(0, 100)]
        protected float castTime = 1f;

        [SerializeField, Range(0, 100)]
        protected float endTime = 1f;

        [SerializeField, Range(0, 100)]
        protected float cooldownTime = 0.5f;

        [Inspector(group = "Phases events", foldable = true)]
        public SkillEvent onBegin;
        public SkillEvent onExecutionStart;
        public SkillEvent onExecutionEnd;
        public SkillEvent onEnd;
        public SkillEvent onCoolDownEnd;

        protected float lastUsed = 0;

        protected CharController user;

        private float timeStarted;

        private Attribute energyAttribute;

        public string Name { get { return this.name; } }
        public bool Using { get; private set; }
        public bool Casting { get; private set; }
        public bool Executing { get; private set; }
        public bool CoolingDown { get; private set; }

        protected virtual void Awake()
        {
            user = GetComponent<CharController>();
            if (this.energyAttribute)
            {
                user.Character.onAttributeInitialized.AddListener(attribute =>
                {
                    if (attribute.Alias.Equals(this.energyAttributeTemplate.Alias))
                        this.energyAttribute = attribute;
                });
            }
            else
            {
                this.energyCost = 0;
            }
        }

        public virtual void Begin()
        {
            Using = true;
            Casting = true;
            this.timeStarted = Time.timeSinceLevelLoad;
            if(this.energyCost > 0)
                this.energyAttribute.Value -= (this.energyCost);

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
            return !Using && !CoolingDown && 
                (this.energyCost == 0 || energyAttribute.Value >= this.energyCost);
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

            yield return WaitHelpers.Sec(cooldownTime);

            CoolingDown = false;

            if (onCoolDownEnd)
                onCoolDownEnd.Invoke(this);
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (this.energyAttributeTemplate && !this.energyAttributeTemplate.HasMaximumValue)
                this.energyAttributeTemplate = null;
        }
#endif

    }
}