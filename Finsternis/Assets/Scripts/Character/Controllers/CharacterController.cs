using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Character), typeof(Movement), typeof(Animator))]
public abstract class CharacterController : MonoBehaviour
{

    protected Animator _characterAnimator;
    protected Movement _characterMovement;

    private bool _locked;

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
        _characterMovement = GetComponent<Movement>();
        _characterAnimator = GetComponent<Animator>();
        GetComponent<Character>().death += CharacterController_death;
    }

    public virtual void Update()
    {
        if(_unlockDelay > 0)
        {
            _unlockDelay--;
            return;
        }

        RaycastHit hit;
        int mask = (1 << LayerMask.NameToLayer("Floor"));
        bool floorBelow = GetComponent<Rigidbody>().velocity.y >= _fallSpeedThreshold || Physics.Raycast(new Ray(transform.position + Vector3.up, Vector3.down), out hit, 4.5f, mask);
        if (floorBelow && _locked && _characterAnimator.GetBool(FallingBool))
        {
            _characterAnimator.SetBool(FallingBool, false);
            Unlock();
        }
        else if (!floorBelow && !_locked)
        {
            Lock();
            _characterAnimator.SetBool(FallingBool, true);
            _characterAnimator.SetFloat(SpeedFloat, 0);
        }

        if (!_locked)
            _characterAnimator.SetFloat(SpeedFloat, _characterMovement.GetHorizontalSpeed());
    }

    public bool IsAttacking()
    {
        return _characterAnimator.GetBool(AttackBool);
    }

    public bool IsDying()
    {
        return _characterAnimator.GetBool(DyingBool);
    }

    public bool IsDead()
    {
        return _characterAnimator.GetBool(DeadBool);
    }

    public bool ShouldWalk()
    {
        return _characterMovement.Direction != Vector3.zero;
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
        _characterAnimator.SetFloat(SpeedFloat, 0);
        _characterMovement.Direction = Vector2.zero;
    }

    public void Unlock(int delay = 0)
    {
        if (delay > 0)
            _unlockDelay = delay;
        
        _locked = false;
    }

    protected virtual void CharacterController_death()
    {
        GetComponent<Character>().death -= CharacterController_death;
        _characterAnimator.SetBool(DyingBool, true);
    }

    private void ActivateBoolean(int booleanHash, int intHash, int type = 0, bool lockMovement = true)
    {
        _characterAnimator.SetBool(booleanHash, true);
        _characterAnimator.SetInteger(intHash, type);

        if (lockMovement)
            _characterMovement.Direction = Vector3.zero;
    }

}
