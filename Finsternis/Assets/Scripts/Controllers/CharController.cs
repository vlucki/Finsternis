using UnityEngine;
using UnityEngine.Events;
using System;
using UnityQuery;

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

        public static readonly int AttackState;
        public static readonly int AttackBool;
        public static readonly int AttackType;
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

        private bool _locked;
        private int _unlockDelay;

        public bool Locked { get { return _locked || _unlockDelay > 0; } }

        static CharController()
        {
            AttackBool  = Animator.StringToHash("attack");
            AttackType  = Animator.StringToHash("attackType");
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
            character = GetComponent<Character>();
        }

        public virtual void Start()
        {
            character.onDeath.AddListener(CharacterController_death);
            GetComponent<Movement>().Speed = GetComponent<Entity>().GetAttribute("spd").Value / 10;
        }

        public virtual void Update()
        {
            if (!IsDead() && !IsDying())
            {
                if (_unlockDelay > 0)
                {
                    _unlockDelay--;
                    return;
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
                if (floorBelow && _locked && characterAnimator.GetBool(FallingBool))
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
                characterMovement.Direction = characterMovement.Direction.WithX(horizontal);
            }
        }

        public void SetZDirection(float vertical)
        {
            if (CanMove())
            {
                characterMovement.Direction = characterMovement.Direction.WithZ(vertical);
            }
        }

        protected virtual void SetDirection(Vector3 direction)
        {
            if (CanMove())
            {
                characterMovement.Direction = direction.WithY(0);
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
            ActivateTrigger(HitBool, HitType, type, lockMovement);
        }

        public virtual void Attack(int type = 0)
        {
            Attack(true, type);
        }

        public virtual void Attack(bool lockMovement, int type = 0)
        {
            if (!CanAttack())
                return;

            ActivateTrigger(AttackBool, AttackType, type, lockMovement);
            onAttack.Invoke(type);
        }

        public virtual bool CanAttack()
        {
            return !IsAttacking();
        }

        public void Lock()
        {
            _locked = true;
            characterAnimator.SetFloat(SpeedFloat, 0);
            characterMovement.Direction = characterMovement.Direction.OnlyY();
        }

        public void Unlock(int delay = 0)
        {
            if (delay > 0)
                _unlockDelay = delay;

            _locked = false;
        }

        protected virtual void CharacterController_death()
        {
            characterAnimator.SetBool(DyingBool, true);
            characterMovement.Direction = Vector2.zero;
        }

        public void ActivateTrigger(int hash, int intHash, int type = 0, bool lockMovement = true)
        {
            characterAnimator.SetTrigger(hash);
            characterAnimator.SetInteger(intHash, type);
            if (lockMovement)
                characterMovement.Direction = Vector3.zero;
        }

        private void ActivateBoolean(int booleanHash, int intHash, int type = 0, bool lockMovement = true)
        {
            characterAnimator.SetBool(booleanHash, true);
            characterAnimator.SetInteger(intHash, type);

            if (lockMovement)
                characterMovement.Direction = Vector3.zero;
        }

    }
}