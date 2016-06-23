using System;
using System.Collections;
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
            StartCoroutine(BeginCasting());
            if (CollingDown)
                StartCoroutine(Cooldown());
        }
    }

    public bool MayUse(int _slot)
    {
        return !CollingDown && _slot == this._slot;
    }

    private IEnumerator BeginCasting()
    {
        if (castTime > 0)
            yield return new WaitForSeconds(castTime);
        CastSkill();
    }

    protected abstract void CastSkill();

    private IEnumerator Cooldown()
    {
        do
        {
            yield return new WaitForEndOfFrame();
            remainingCooldown -= Time.deltaTime;
        } while (CollingDown);
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}