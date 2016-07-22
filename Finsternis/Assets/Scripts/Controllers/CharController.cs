using UnityEngine;
using UnityEngine.Events;
using System;
using UnityQuery;
using System.Collections.Generic;

namespace Finsternis
{
    [AddComponentMenu("Finsternis/Char Controller")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Character), typeof(Movement), typeof(Animator))]
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
        protected Movement characterMovement;

        [SerializeField]
        [Range(0, -1)]
        private float fallSpeedThreshold = -0.2f;

        [Range(0, 1)]
        [SerializeField]
        private float turningSpeed = 0.05f;

        [SerializeField]
        private List<Skill> skills;

        [SerializeField]
        private Skill[] equippedSkills = new Skill[4];

        private bool locked;
        private float unlockDelay;
        private bool waitingForDelay;

        public bool Locked { get { return this.locked; } }
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
            this.locked = false;
            characterMovement = GetComponent<Movement>();
            characterAnimator = GetComponent<Animator>();
            character         = GetComponent<Character>();
        }

        public virtual void Start()
        {
            character.onDeath.AddListener(CharacterController_death);
            characterMovement.Speed = character.GetAttribute("spd").Value / 10;
            Array.ForEach<Skill>(this.equippedSkills, (skill) => { if(skill) skill.Equip(); }); //make sure every skill that is equipped knows it
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

                if (!locked)
                {
                    UpdateRotation();
                    characterAnimator.SetFloat(CharController.SpeedFloat, characterMovement.VelocityNoY());
                }
            }
        }

        public virtual void FixedUpdate()
        {
            if (!IsDead())
            {
                RaycastHit hit;
                int mask = (1 << LayerMask.NameToLayer("Floor"));
                bool floorBelow = 
                    GetComponent<Rigidbody>().velocity.y >= fallSpeedThreshold 
                    || Physics.Raycast(new Ray(transform.position + Vector3.up, Vector3.down), out hit, 4.25f, mask);

                if (floorBelow && this.locked && IsFalling() && !this.waitingForDelay)
                {
                    characterAnimator.SetBool(CharController.FallingBool, false);
                    Unlock();
                }
                else if (!floorBelow && !this.locked)
                {
                    Lock();
                    characterAnimator.SetBool(CharController.FallingBool, true);
                    characterAnimator.SetFloat(CharController.SpeedFloat, 0);
                }
            }
        }

        public void SetXDirection(float horizontal)
        {
            if (CanMove())
            {
                characterMovement.SetDirection(characterMovement.Direction.WithX(horizontal));
            }
        }

        public void SetZDirection(float vertical)
        {
            if (CanMove())
            {
                characterMovement.SetDirection(characterMovement.Direction.WithZ(vertical));
            }
        }

        protected virtual void SetDirection(Vector3 direction)
        {
            if (CanMove())
            {
                characterMovement.SetDirection(direction.WithY(0));
            }
        }

        private void UpdateRotation()
        {
            transform.forward = Vector3.Slerp(
                transform.forward, 
                characterMovement.LastDirection, 
                Mathf.Max(this.turningSpeed, Vector3.Angle(transform.forward, characterMovement.Direction) / 720));
        }

        protected virtual bool CanMove()
        {
            bool staggered = IsStaggered();
            bool attacking = IsAttacking();

            return !(this.locked || staggered || attacking);
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
            return characterMovement.Direction != Vector3.zero;
        }

        public virtual void Hit(int type = 0, bool lockMovement = true)
        {
            this.characterAnimator.SetInteger(HitType, type);
            this.characterAnimator.SetTrigger(HitTrigger);

            if (lockMovement)
                Lock();
        }

        public virtual void Attack(int slot = 0)
        {
            if (!CanAttack())
            {
#if UNITY_EDITOR
                print(ToString() + " can't attack right now");
#endif
                return;
            }

            if (!ValidateSkillSlot(slot))
                return;

            if (this.equippedSkills[slot].Use())
            {
                characterAnimator.SetInteger(AttackSlot, slot);
                onAttack.Invoke(slot);
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
            this.locked = true;
            characterAnimator.SetFloat(CharController.SpeedFloat, 0);
            characterMovement.SetDirection(characterMovement.Direction.OnlyY());
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
            this.locked = false;
            onUnlock.Invoke();
        }

        protected virtual void CharacterController_death()
        {
            characterAnimator.SetBool(CharController.DyingBool, true);
            characterMovement.SetDirection(Vector3.zero);
        }
    }
}