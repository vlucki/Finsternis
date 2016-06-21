using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class Skill : MonoBehaviour
{
    [SerializeField]
    [Range(0, 5)]
    protected float castTime = 0.2f;

    [SerializeField]
    [Range(0, 5)]
    protected float cooldownTime = 0.5f;

    protected float remainingCooldown = 0;

    public bool CollingDown { get { return remainingCooldown > 0; } }

    void Awake()
    {
        GetComponent<CharacterController>().onAttack.AddListener(Use);
    }

    private void Use()
    {
        if (!CollingDown)
        {
            remainingCooldown = cooldownTime;
            StartCoroutine(BeginCasting());
            if (CollingDown)
                StartCoroutine(Cooldown());
        }
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
}