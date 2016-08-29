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

        [SerializeField][ReadOnly]
        private bool couldBeFalling = false;

        [Range(0, 1)]
        [SerializeField]
        private float turningSpeed = 0.05f;

        [SerializeField]
        private List<Skill> skills;

        [SerializeField]
        private Skill[] equippedSkills = new Skill[4];

        private bool actionsLocked;
        private float unlockDelay;
        private bool waitingForDelay;

        public bool ActionsLocked { get { return this.actionsLocked; } }
        public Character Character { get { return character; } }

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
                Log.Error("Exception thrown when initializing controller for " + gameObject);
                throw e;
            }
        }

        public virtual void Update()
        {
            if (!IsDead() && !IsDying())
            {
                if (this.waitingForDelay)
                {
                    if (this.unlockDelay > 0)
                        this.unlockDelay -= Time.deltaTime;
                    else
                        Unlock();
                }

                if (!actionsLocked)
                {
                    UpdateRotation();
                    characterAnimator.SetFloat(CharController.SpeedFloat, characterMovement.GetVelocityMagnitude());
                }
            }
        }

        private bool UpdateFallingState()
        {
            bool wasFalling = this.couldBeFalling;
            float fallingSpeed = GetComponent<Rigidbody>().velocity.y;
            this.couldBeFalling = (fallingSpeed <= fallSpeedThreshold);

            if (wasFalling == this.couldBeFalling)
                this.fallingStateCheckCount++;
            else
                this.fallingStateCheckCount = 1;

            bool isFallingNow = (this.fallingStateCheckCount == this.fallingStateChecks);

            return isFallingNow;
        }

        public virtual void FixedUpdate()
        {
            if (!IsDead())
            {
                if(UpdateFallingState())
                {
                    if (!this.couldBeFalling && this.actionsLocked && IsFalling() && !this.waitingForDelay)
                    {
                        characterAnimator.SetBool(CharController.FallingBool, false);
                        Unlock();
                    }
                    else if (this.couldBeFalling && !this.actionsLocked)
                    {
                        Lock();
                        characterAnimator.SetBool(CharController.FallingBool, true);
                        characterAnimator.SetFloat(CharController.SpeedFloat, 0);
                    }
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

        private void UpdateRotation()
        {
            float currentVelocity = characterMovement.GetVelocityMagnitude();
            if (currentVelocity <= 0.1f)
                return;
            transform.forward = Vector3.Slerp(
                transform.forward, 
                characterMovement.Velocity, 
                turningSpeed);
        }

        protected virtual bool CanMove()
        {
            bool staggered = IsStaggered();
            bool attacking = IsAttacking();

            return !(this.actionsLocked || staggered || attacking);
        }

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

        public bool ShouldWalk()
        {
            return !characterMovement.Direction.IsZero();
        }

        public virtual void Hit(int type = 0, bool lockMovement = true)
        {
            if (this.character.Invincible)
                return;

            this.characterAnimator.SetInteger(HitType, type);
            this.characterAnimator.SetTrigger(HitTrigger);

            if (lockMovement)
                Lock();
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
                this.equippedSkills[(int)slot].Use();
                characterAnimator.SetInteger(AttackSlot, (int)slot);
                characterAnimator.SetTrigger(AttackTrigger);
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
                Log.Error("Variable 'equippedSkills' not initialized.");
                return false;
            }
            else if (slot > this.equippedSkills.Length || slot < 0)
            {
                Log.Error("Invalid skill slot (" + slot + ")");
                return false;
            }
            else if(checkForEmptySlot && !this.equippedSkills[slot])
            {
                Log.Warn("No skill equipped in slot " + slot);
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
    }
}