using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Character), typeof(Movement), typeof(Animator))]
public abstract class CharacterController : MonoBehaviour {

    protected Animator _characterAnimator;
    protected Movement _characterMovement;

    public static readonly int AttackState;
    public static readonly int AttackBool;
    public static readonly int AttackType;
    public static readonly int SpeedFloat;
    public static readonly int DyingBool;
    public static readonly int DeadBool;
    public static readonly int HitBool;
    public static readonly int HitType;

    static CharacterController()
    {
        AttackBool = Animator.StringToHash("attacking");
        AttackType = Animator.StringToHash("attackType");
        SpeedFloat = Animator.StringToHash("speed");
        DyingBool = Animator.StringToHash("dying");
        DeadBool = Animator.StringToHash("dead");
        HitBool = Animator.StringToHash("hit");
        HitType = Animator.StringToHash("hitType");

    }

    public virtual void Awake ()
    {
        _characterMovement = GetComponent<Movement>();
        _characterAnimator = GetComponent<Animator>();
        GetComponent<Character>().death += CharacterController_death;
    }

    public virtual void Update()
    {
        _characterAnimator.SetFloat(SpeedFloat, _characterMovement.GetSpeed());
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

    private void ActivateBoolean(int booleanHash, int intHash, int type = 0, bool lockMovement = true)
    {
        _characterAnimator.SetBool(booleanHash, true);
        _characterAnimator.SetInteger(intHash, type);

        if (lockMovement)
            _characterMovement.Direction = Vector3.zero;
    }

    public virtual void Hit(int type = 0, bool lockMovement = true)
    {
        ActivateBoolean(HitBool, HitType, type, lockMovement);
    }

    public virtual void Attack(int type = 0, bool lockMovement = true)
    {
        ActivateBoolean(AttackBool, AttackType, type, lockMovement);
    }

    protected virtual void CharacterController_death()
    {
        GetComponent<Character>().death -= CharacterController_death;
        _characterAnimator.SetBool(DyingBool, true);
    }

}
