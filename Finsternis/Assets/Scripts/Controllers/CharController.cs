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

        protected Character character;
        protected Animator characterAnimator;
        protected Movement characterMovement;

        public AttackEvent onAttack;
        public UnityEvent onLock;
        public UnityEvent onUnlock;
        
        public static readonly int AttackBool;
        public static readonly int AttackType;
        public static readonly int AttackSpeed;
        public static readonly int DyingBool;
        public static readonly int DeadBool;
        public static readonly int FallingBool;
        public static readonly int HitBool;
        public static readonly int HitType;
        public static readonly int SpeedFloat;

        [SerializeField]
        [Range(0, -1)]
        private float _fallSpeedThreshold = -0.2f;

        [Range(0, 1)]
        [SerializeField]
        private float _turningSpeed = 0.05f;

        [SerializeField]
        private List<Skill> skills;

        [SerializeField]
        private Skill[] equippedSkills = new Skill[4];

        private bool _locked;
        private float _unlockDelay;
        private bool _waitingForDelay;

        public bool Locked { get { return _locked || _unlockDelay > 0; } }
        public Character Character { get { return character; } }

        static CharController()
        {
            AttackBool  = Animator.StringToHash("attack");
            AttackType  = Animator.StringToHash("attackType");
            AttackSpeed = Animator.StringToHash("attackSpeed");
            DyingBool   = Animator.StringToHash("dying");
            DeadBool    = Animator.StringToHash("dead");
            FallingBool = Animator.StringToHash("falling");
            HitBool     = Animator.StringToHash("hit");
            HitType     = Animator.StringToHash("hitType");
            SpeedFloat  = Animator.StringToHash("speed");

        }

        public virtual void Awake()
        {
            _locked = false;
            characterMovement = GetComponent<Movement>();
            characterAnimator = GetComponent<Animator>();
            character         = GetComponent<Character>();
        }

        public virtual void Start()
        {
            character.onDeath.AddListener(CharacterController_death);
            characterMovement.Speed = character.GetAttribute("spd").Value / 10;
            Array.ForEach<Skill>(equippedSkills, (skill) => { if(skill) skill.Equip(); }); //make sure every skill that is equipped knows it
        }

        public virtual void Update()
        {
            if (!IsDead() && !IsDying())
            {
                if (_waitingForDelay)
                {
                    if (_unlockDelay > 0)
                        _unlockDelay -= Time.deltaTime;
                    else
                        Unlock();
                }

                if (!_locked)
                {
                    UpdateRotation();
                    characterAnimator.SetFloat(SpeedFloat, characterMovement.VelocityNoY());
                }
            }
        }

        public virtual void FixedUpdate()
        {
            if (!IsDead())
            {
                RaycastHit hit;
                int mask = (1 << LayerMask.NameToLayer("Floor"));
                bool floorBelow = GetComponent<Rigidbody>().velocity.y >= _fallSpeedThreshold || Physics.Raycast(new Ray(transform.position + Vector3.up, Vector3.down), out hit, 4.25f, mask);
                if (floorBelow && _locked && characterAnimator.GetBool(FallingBool) && !_waitingForDelay)
                {
                    characterAnimator.SetBool(FallingBool, false);
                    Unlock();
                }
                else if (!floorBelow && !_locked)
                {
                    Lock();
                    characterAnimator.SetBool(FallingBool, true);
                    characterAnimator.SetFloat(SpeedFloat, 0);
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
            transform.forward = Vector3.Slerp(transform.forward, characterMovement.LastDirection, Mathf.Max(_turningSpeed, Vector3.Angle(transform.forward, characterMovement.Direction) / 720));
        }

        protected virtual bool CanMove()
        {
            bool staggered = IsStaggered();
            bool attacking = IsAttacking();

            return !(_locked || staggered || attacking);
        }

        public bool IsAttacking()
        {
            return characterAnimator.GetBool(AttackBool);
        }

        public bool IsDying()
        {
            return characterAnimator.GetBool(DyingBool);
        }

        public bool IsDead()
        {
            return characterAnimator.GetBool(DeadBool);
        }

        public bool IsFalling()
        {
            return characterAnimator.GetBool(FallingBool);
        }

        public bool IsStaggered()
        {
            return characterAnimator.GetBool(HitBool);
        }

        public bool ShouldWalk()
        {
            return characterMovement.Direction != Vector3.zero;
        }

        public virtual void Hit(int type = 0, bool lockMovement = true)
        {
            ActivateTrigger(HitBool, HitType, type);
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
            if(equippedSkills == null)
            {
                Log.Error("No skill equipped");
                return;
            }
            else if(slot > equippedSkills.Length || slot < 0 )
            {
                Log.Error("Invalid skill slot (" + slot + ")");
                return;
            }
            equippedSkills[slot].Use();
            onAttack.Invoke(slot);
        }

        public void EquipSkill(Skill skill, int slot)
        {
            if (slot > equippedSkills.Length || slot < 0)
            {
                Log.Error("Invalid skill slot (" + slot + ")");
                return;
            }

            if (equippedSkills[slot])
                equippedSkills[slot].Unequip();

            equippedSkills[slot] = skill;
            skill.Equip();
        }

        public virtual bool CanAttack()
        {
            return !IsAttacking();
        }

        public void Lock()
        {
            _locked = true;
            characterAnimator.SetFloat(SpeedFloat, 0);
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
            _waitingForDelay = true;
            _unlockDelay = delay;
        }

        public void Unlock()
        {
            _waitingForDelay = false;
            _locked = false;
            onUnlock.Invoke();
        }

        protected virtual void CharacterController_death()
        {
            characterAnimator.SetBool(DyingBool, true);
            characterMovement.SetDirection(Vector3.zero);
        }

        public void ActivateTrigger(int hash, int intHash, int type = 0)
        {
            characterAnimator.SetTrigger(hash);
            characterAnimator.SetInteger(intHash, type);
        }

        private void ActivateBoolean(int booleanHash, int intHash, int type = 0)
        {
            characterAnimator.SetBool(booleanHash, true);
            characterAnimator.SetInteger(intHash, type);
        }

    }
}