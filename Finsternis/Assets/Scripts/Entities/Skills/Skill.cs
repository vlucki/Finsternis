using MovementEffects;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class Skill : MonoBehaviour
{
    [SerializeField]
    private int _slot;

    [SerializeField]
    [Range(0, 5)]
    protected float castTime = 0.2f;

    [SerializeField]
    [Range(0, 5)]
    protected float cooldownTime = 0.5f;

    protected float remainingCooldown = 0;
    private IEnumerator<float> castingHandle;
    private IEnumerator<float> cooldownHandle;

    public bool CollingDown { get { return remainingCooldown > 0; } }

    protected virtual void Awake()
    {
        GetComponent<CharacterController>().onAttack.AddListener(Use);
    }

    protected virtual void Use(int _slot)
    {
        if (MayUse(_slot))
        {
            remainingCooldown = cooldownTime;
            castingHandle = Timing.RunCoroutine(_BeginCasting());
            if (CollingDown)
                cooldownHandle = Timing.RunCoroutine(_Cooldown());
        }
    }

    public bool MayUse(int _slot)
    {
        return !CollingDown && _slot == this._slot;
    }

    private IEnumerator<float> _BeginCasting()
    {
        if (castTime > 0)
            yield return Timing.WaitForSeconds(castTime);
        CastSkill();
    }

    protected abstract void CastSkill();

    private IEnumerator<float> _Cooldown()
    {
        do
        {
            yield return 0f;
            remainingCooldown -= Time.deltaTime;
        } while (CollingDown);
    }

    void OnDisable()
    {
        Timing.KillCoroutine(castingHandle);
        Timing.KillCoroutine(cooldownHandle);
    }

    void OnDestroy()
    {
        Timing.KillCoroutine(castingHandle);
        Timing.KillCoroutine(cooldownHandle);
    }
}