using UnityEngine;
using System.Collections;

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
        GameObject summonedFireball = Instantiate(fireballPrefab, summonPoint.position + transform.forward*summonOffset, transform.rotation) as GameObject;
        summonedFireball.GetComponent<PhysicalAttackHandler>().ignoreColliders.Add(GetComponent<Collider>());
        summonedFireball.GetComponent<PhysicalAttackHandler>().owner = GetComponent<Entity>();
        summonedFireball.SetActive(true);
        StartCoroutine(Shoot(summonedFireball));
    }

    private IEnumerator Shoot(GameObject summonedFireball)
    {
        yield return new WaitForFixedUpdate();
        summonedFireball.GetComponent<Rigidbody>().AddForce(summonedFireball.transform.forward * 50, ForceMode.Impulse);
    }
}
