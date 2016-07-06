using UnityEngine;
using System.Collections.Generic;
using MovementEffects;

public class Fireball : Skill
{
    public GameObject fireballPrefab;
    public Transform summonPoint;
    public float summonOffset;

    protected override void Use(int _slot)
    {
        if(MayUse(_slot))
        {
            base.Use(_slot);
            GetComponent<Animator>().SetFloat("attackSpeed", 5f);
        }
    }

    protected override void CastSkill()
    {
        GameObject summonedFireball = Instantiate(fireballPrefab, summonPoint.position + transform.forward * summonOffset, transform.rotation) as GameObject;
        PhysicalAttackHandler pah = summonedFireball.GetComponent<PhysicalAttackHandler>();
        pah.ignoreColliders.Add(GetComponent<Collider>());
        pah.owner = GetComponent<Entity>();
        summonedFireball.SetActive(true);
        Timing.RunCoroutine(_Shoot(summonedFireball), Segment.FixedUpdate);
    }

    private IEnumerator<float> _Shoot(GameObject summonedFireball)
    {
        summonedFireball.GetComponent<Rigidbody>().AddForce(summonedFireball.transform.forward * 50, ForceMode.Impulse);
        yield return 0f;
    }
}
