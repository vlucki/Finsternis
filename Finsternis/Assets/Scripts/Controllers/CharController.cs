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
        public static readonly int AttackSlot;
        public static readonly int AttackSpeed;
        public static readonly int DyingBool;
        public static readonly int DeadBool;
        public static readonly int FallingBool;
        public static readonly int HitTrigger;
        public static readonly int HitType;
        public static readonly int SpeedFloat;

        public AttackEvent onAttack;
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

        private bool actionsLocked;
        private float unlockDelay;
        private bool waitingForDelay;

        public bool ActionsLocked { get { return this.actionsLocked; } }
        public Character Character { get { return character; } }
        public Skill[] EquippedSkills { get { return this.equippedSkills; } }

        static CharController()
        {
            AttackTrigger = Animator.StringToHash("attack");
            AttackSlot    = Animator.StringToHash("attackSlot");
            AttackSpeed   = Animator.StringToHash("attackSpeed");
            DyingBool     = Animator.StringToHash("dying");
            DeadBool      = Animator.StringToHash("dead");
            FallingBool   = Animator.StringToHash("falling");
            HitTrigger    = Animator.StringToHash("hit");
            HitType       = Animator.StringToHash("hitType");
            SpeedFloat    = Animator.StringToHash("speed");
        }

        public virtual void Awake()
        {
            this.actionsLocked = false;
            characterMovement = GetComponent<MovementAction>();
            characterAnimator = GetComponent<Animator>();
            character         = GetComponent<Character>();
        }

        public virtual void Start()
        {
            try
            {
                character.onDeath.AddListener(CharacterController_death);
                Array.ForEach<Skill>(this.equippedSkills, (skill) => { if (skill) skill.Equip(); }); //make sure every skill that is equipped knows it

            }
            catch (Exception e)
            {
                Log.Error(this, "Exception thrown when initializing controller for character!");
                throw e;
            }
        }

        public virtual void Update()
        {
            if (!IsDead() && !IsDying())
            {
                if (this.actionsLocked)
                {
                    if (this.waitingForDelay)
                    {
                        if (this.unlockDelay > 0)
                            this.unlockDelay -= Time.deltaTime;
                        else if(!IsFalling())
                            Unlock();
                    }
                    else if (!IsFalling())
                        Unlock();
                }
                else
                {
                    if (IsFalling())
                        Lock();
                    else
                        this.characterAnimator.SetFloat(CharController.SpeedFloat, this.characterMovement.GetVelocityMagnitude());
                }
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
            if (!IsDead())
            {
                if (ShouldUpdateFallingState())
                {
                    bool falling = !IsFalling();
                    characterAnimator.SetBool(CharController.FallingBool, falling);
                }
            }
        }

        public void SetXDirection(float amount)
        {
            if (CanMove())
            {
                characterMovement.Direction = (characterMovement.Direction.WithX(amount));
            }
        }

        public void SetZDirection(float amount)
        {
            if (CanMove())
            {
                characterMovement.Direction = (characterMovement.Direction.WithZ(amount));
            }
        }

        protected virtual void SetDirection(Vector3 direction)
        {
            if (CanMove())
            {
                characterMovement.Direction = (direction.WithY(0));
            }
        }

        public bool ShouldWalk()
        {
            return !characterMovement.Direction.IsZero();
        }

        protected virtual bool CanMove()
        {
            bool staggered = IsStaggered();
            bool attacking = IsAttacking();

            return !(this.actionsLocked || staggered || attacking);
        }

        #region Shorthand for booleans in mecanim
        public bool IsAttacking()
        {
            return characterAnimator.GetBool(CharController.AttackTrigger);
        }

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

        public bool IsStaggered()
        {
            return characterAnimator.GetBool(CharController.HitTrigger);
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
            if (!CanAttack())
            {
#if UNITY_EDITOR
                print(ToString() + " can't attack right now");
#endif
                return;
            }

            if (!ValidateSkillSlot((int)slot))
                return;

            if (this.equippedSkills[(int)slot].MayUse())
            {
                characterAnimator.SetInteger(AttackSlot, (int)slot);
                characterAnimator.SetTrigger(AttackTrigger);
                this.equippedSkills[(int)slot].Use();
            }
        }

        public void ExecuteSkill(Skill skill)
        {
            int slot = -1;
            for (slot = 0; slot < this.equippedSkills.Length; slot++)
                if (this.equippedSkills[slot].Equals(skill))
                    break;
            if (slot >= 0 && slot < this.equippedSkills.Length)
            {
                onAttack.Invoke((int)slot);
            }
        }

        public void EquipSkill(Skill skill, int slot)
        {
            if (!ValidateSkillSlot(slot, false))
                return;

            if (equippedSkills[slot])
                equippedSkills[slot].Unequip();

            equippedSkills[slot] = skill;
            skill.Equip();
        }

        private bool ValidateSkillSlot(int slot, bool checkForEmptySlot = true)
        {
            if (this.equippedSkills == null)
            {
                Log.Error(this, "Variable 'equippedSkills' not initialized.");
                return false;
            }
            else if (slot > this.equippedSkills.Length || slot < 0)
            {
                Log.Error(this, "Invalid skill slot: {0}", slot);
                return false;
            }
            else if(checkForEmptySlot && !this.equippedSkills[slot])
            {
                Log.Warn(this, "No skill equipped in slot {0}", slot);
                return false;
            }
            return true;
        }

        public virtual bool CanAttack()
        {
            return !IsAttacking();
        }

        public void Lock()
        {
            this.actionsLocked = true;
            characterAnimator.SetFloat(CharController.SpeedFloat, 0);
            characterMovement.Direction = (characterMovement.Direction.OnlyY());
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
            unlockDelay = delay;
        }

        public void Unlock()
        {
            this.waitingForDelay = false;
            this.actionsLocked = false;
            onUnlock.Invoke();
        }

        protected virtual void CharacterController_death()
        {
            characterAnimator.SetBool(CharController.DyingBool, true);
            characterMovement.Direction = Vector3.zero;
        }

        void OnValidate()
        {
            GetComponents<Skill>(this.skills);
            if(this.equippedSkills.Length != 4)
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
            }
        }
    }
}