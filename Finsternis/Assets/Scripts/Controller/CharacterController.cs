using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Entity), typeof(Movement), typeof(Animator))]
public abstract class CharacterController : MonoBehaviour
{
    protected Entity character;
    protected Animator characterAnimator;
    protected Movement characterMovement;

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

    private bool _locked;
    private int _unlockDelay;

    public bool Locked { get { return _locked || _unlockDelay > 0; } }

    static CharacterController()
    {
        AttackBool = Animator.StringToHash("attacking");
        AttackType = Animator.StringToHash("attackType");
        DyingBool = Animator.StringToHash("dying");
        DeadBool = Animator.StringToHash("dead");
        FallingBool = Animator.StringToHash("falling");
        HitBool = Animator.StringToHash("hit");
        HitType = Animator.StringToHash("hitType");
        SpeedFloat = Animator.StringToHash("speed");

    }

    public virtual void Awake()
    {
        _locked = false;
        characterMovement = GetComponent<Movement>();
        characterAnimator = GetComponent<Animator>();
        character = GetComponent<Entity>();
    }

    public virtual void Start()
    {
        character.onDeath.AddListener(CharacterController_death);
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

            RaycastHit hit;
            int mask = (1 << LayerMask.NameToLayer("Floor"));
            bool floorBelow = GetComponent<Rigidbody>().velocity.y >= _fallSpeedThreshold || Physics.Raycast(new Ray(transform.position + Vector3.up, Vector3.down), out hit, 4.5f, mask);
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

            if (!_locked)
                characterAnimator.SetFloat(SpeedFloat, characterMovement.GetHorizontalSpeed());
        }
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

    public bool ShouldWalk()
    {
        return characterMovement.Direction != Vector3.zero;
    }

    public virtual void Hit(int type = 0, bool lockMovement = true)
    {
        ActivateBoolean(HitBool, HitType, type, lockMovement);
    }

    public virtual void Attack(int type = 0, bool lockMovement = true)
    {
        ActivateBoolean(AttackBool, AttackType, type, lockMovement);
    }

    public void Lock()
    {
        _locked = true;
        characterAnimator.SetFloat(SpeedFloat, 0);
        characterMovement.Direction = Vector2.zero;
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

    private void ActivateBoolean(int booleanHash, int intHash, int type = 0, bool lockMovement = true)
    {
        characterAnimator.SetBool(booleanHash, true);
        characterAnimator.SetInteger(intHash, type);

        if (lockMovement)
            characterMovement.Direction = Vector3.zero;
    }

}
