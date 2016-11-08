using UnityEngine;
using UnityEngine.Events;
using System;
using UnityQuery;
using System.Collections.Generic;

namespace Finsternis
{
    [AddComponentMenu("Finsternis/Char Controller")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Character), typeof(MovementAction), typeof(Animator))]
    public class CharController : MonoBehaviour
    {

        [Serializable]
        public class AttackEvent : UnityEvent<int> { }

        public static readonly int AttackTrigger;
        public static readonly int AttackSpeed;
        public static readonly int DyingBool;
        public static readonly int DeadBool;
        public static readonly int FallingBool;
        public static readonly int HitTrigger;
        public static readonly int HitType;
        public static readonly int SpeedFloat;

        //public AttackEvent onAttack;
        public UnityEvent onLock;
        public UnityEvent onUnlock;

        protected Character character;

        protected Animator characterAnimator;
        protected MovementAction characterMovement;

        [SerializeField]
        [Range(0, -1)]
        private float fallSpeedThreshold = -0.2f;

        [SerializeField]
        [Range(0, 10)]
        [Tooltip("How many updates to consider before changing the 'falling state'")]
        private int fallingStateChecks = 3;

        [SerializeField]
        [ReadOnly]
        private int fallingStateCheckCount;

        [SerializeField]
        private List<Skill> skills;

        [SerializeField]
        private Skill[] equippedSkills = new Skill[4];

        private bool isLocked;
        private bool waitingForDelay;
        private Coroutine unlockDelayedCall;

        public bool IsLocked { get { return this.isLocked; } }
        public Character Character { get { return character; } }
        public Animator Controller { get { return characterAnimator; } }
        public Skill[] EquippedSkills { get { return this.equippedSkills; } }
        public Skill ActiveSkill { get; private set; }

        static CharController()
        {
            AttackTrigger = Animator.StringToHash("attack");
            AttackSpeed = Animator.StringToHash("attackSpeed");
            DyingBool = Animator.StringToHash("dying");
            DeadBool = Animator.StringToHash("dead");
            FallingBool = Animator.StringToHash("falling");
            HitTrigger = Animator.StringToHash("hit");
            HitType = Animator.StringToHash("hitType");
            SpeedFloat = Animator.StringToHash("speed");
        }

        public virtual void Awake()
        {
            this.isLocked = false;
            characterMovement = GetComponent<MovementAction>();
            characterAnimator = GetComponent<Animator>();
            character = GetComponent<Character>();
        }

        public virtual void Start()
        {
            try
            {
                character.onDeath.AddListener(CharacterController_death);
            }
            catch (Exception e)
            {
                Log.E(this, "Exception thrown when initializing controller for character!");
                throw e;
            }
        }

        public virtual void Update()
        {
            if (this.ActiveSkill)
                return;

            if (!IsDead() && !IsDying())
            {
                if (this.isLocked)
                {
                    if (!this.waitingForDelay && !IsFalling())
                        Unlock();
                }

                if (IsFalling())
                    Lock();
                else if(CanAct())
                    this.characterAnimator.SetFloat(CharController.SpeedFloat, this.characterMovement.GetVelocityMagnitude());

            }
        }

        /// <summary>
        /// Verifies wheter the character is falling.
        /// </summary>
        /// <returns>True if character is actually falling.</returns>
        private bool ShouldUpdateFallingState()
        {
            bool wasFalling = IsFalling(); //stores current state
            float fallingSpeed = this.characterMovement.Velocity.y;

            //compares Y velocity to threshold to account for some variations due to how the physics engine work
            bool couldBeFalling = (fallingSpeed <= this.fallSpeedThreshold);

            //if the state is consistent (that is, the character was falling and, apparently, still is)
            if (couldBeFalling)
            {
                if (this.fallingStateChecks > this.fallingStateCheckCount)
                    this.fallingStateCheckCount++; //update the counter
            }
            else
                this.fallingStateCheckCount = 0;

            //use the counter in order to define if the character is actually falling
            //this is used in order to account for the tiny variations that may occur from frame to frame
            //i.e. to avoid thinking a "physics hiccup" meant the character was falling
            bool isFallingNow = (this.fallingStateCheckCount == this.fallingStateChecks);

            return isFallingNow != wasFalling;
        }

        public virtual void FixedUpdate()
        {
            if (!this.ActiveSkill && !IsDead())
            {
                if (ShouldUpdateFallingState())
                {
                    characterAnimator.SetBool(CharController.FallingBool, !IsFalling());
                }
            }
        }

        public void SetXDirection(float amount)
        {
            if (CanAct())
            {
                characterMovement.MovementDirection = (characterMovement.MovementDirection.WithX(amount));
            }
        }

        public void SetZDirection(float amount)
        {
            if (CanAct())
            {
                characterMovement.MovementDirection = (characterMovement.MovementDirection.WithZ(amount));
            }
        }

        protected virtual void SetDirection(Vector3 direction)
        {
            if (CanAct())
            {
                characterMovement.MovementDirection = (direction.WithY(0));
            }
        }

        public bool ShouldWalk()
        {
            return !characterMovement.MovementDirection.IsZero();
        }

        protected virtual bool CanAct()
        {
            return this.isActiveAndEnabled && !this.isLocked;
        }

        #region Shorthand for booleans in mecanim

        public bool IsDying()
        {
            return characterAnimator.GetBool(CharController.DyingBool);
        }

        public bool IsDead()
        {
            return characterAnimator.GetBool(CharController.DeadBool);
        }

        public bool IsFalling()
        {
            return characterAnimator.GetBool(CharController.FallingBool);
        }

        #endregion

        public virtual void Hit(int type = 0)
        {
            if (this.character.Invincible)
                return;

            this.characterAnimator.SetInteger(HitType, type);
            this.characterAnimator.SetTrigger(HitTrigger);
        }

        public virtual void Attack(float slot = 0)
        {
            if (!CanAct())
            {
#if LOG_INFO || LOG_WARN
                Log.W(this, " can't attack right now");
#endif
                return;
            }

            if (!ValidateSkillSlot((int)slot))
                return;

            if (this.equippedSkills[(int)slot].MayUse())
            {
                this.Controller.SetTrigger(AttackTrigger);
                ActiveSkill = this.equippedSkills[(int)slot];
                ActiveSkill.onEnd.AddListener(SkillCastEnd);
                ActiveSkill.Begin();
            }
        }

        private void SkillCastEnd(Skill skill)
        {
            ActiveSkill = null;
            skill.onEnd.RemoveListener(SkillCastEnd);
        }

        public void EquipSkill(Skill skill, int slot)
        {
            if (!ValidateSkillSlot(slot, false))
                return;
            
            equippedSkills[slot] = skill;
        }

        private bool ValidateSkillSlot(int slot, bool checkForEmptySlot = true)
        {
            if (this.equippedSkills == null)
            {
                Log.E(this, "Variable 'equippedSkills' not initialized.");
                return false;
            }
            else if (slot > this.equippedSkills.Length || slot < 0)
            {
                Log.E(this, "Invalid skill slot: {0}", slot);
                return false;
            }
            else if (checkForEmptySlot && !this.equippedSkills[slot])
            {
#if LOG_INFO || LOG_WARN
                Log.W(this, "No skill equipped in slot {0}", slot);
#endif
                return false;
            }
            return true;
        }

        public void Lock()
        {
            this.isLocked = true;
            characterAnimator.SetFloat(CharController.SpeedFloat, 0);
            characterMovement.MovementDirection = (characterMovement.MovementDirection.OnlyY());
            onLock.Invoke();
        }

        public void LockAndDisable()
        {
            Lock();
            this.Disable();
        }

        public void Lock(float duration)
        {
            Lock();
            UnlockWithDelay(duration);
        }

        public void UnlockWithDelay(float delay)
        {
            this.waitingForDelay = true;
            this.unlockDelayedCall = this.CallDelayed(delay, Unlock);
        }

        public void Unlock()
        {
            if (!this.isActiveAndEnabled)
                return;

            if (waitingForDelay)
            {
                this.StopCoroutine(this.unlockDelayedCall);
                this.waitingForDelay = false;
            }

            this.isLocked = false;
            onUnlock.Invoke();
        }

        protected virtual void CharacterController_death()
        {
            characterAnimator.SetBool(CharController.DyingBool, true);
            characterMovement.MovementDirection = Vector3.zero;
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            ValidateSkills();
        }

        private void ValidateSkills()
        {
            GetComponents<Skill>(this.skills);
            if (this.equippedSkills.Length != 4)
            {
                var tmp = this.equippedSkills;
                this.equippedSkills = new Skill[4];
                for (int i = 0; i < tmp.Length && i < 4; i++)
                    this.equippedSkills[i] = tmp[i];
            }

            for (int i = 0; i < 4; i++)
            {
                if (!this.skills.Contains(this.equippedSkills[i]))
                    this.equippedSkills[i] = null;
                else
                {
                    for(int j = 0; j < 4; j++)
                    {
                        if (i != j && this.equippedSkills[i] == this.equippedSkills[j])
                            this.equippedSkills[j] = null;
                    }
                }
            }
        }
#endif
    }
}