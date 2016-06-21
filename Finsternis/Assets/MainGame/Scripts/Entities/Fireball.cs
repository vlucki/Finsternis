using UnityEngine;
using System.Collections;
using System;

public class Fireball : Skill
{
    public GameObject fireballPrefab;
    public Transform summonPoint;
    public Vector3 summonOffset;


    protected override void CastSkill()
    {
        GameObject summonedFireball = Instantiate(fireballPrefab, summonPoint.position + summonOffset, transform.rotation) as GameObject;
        summonedFireball.GetComponent<PhysicalAttackHandler>().ignoreColliders.Add(GetComponent<Collider>());
        summonedFireball.GetComponent<PhysicalAttackHandler>().owner = GetComponent<Entity>();
        summonedFireball.GetComponent<Rigidbody>().velocity = (summonedFireball.transform.forward * 10);
    }
}
