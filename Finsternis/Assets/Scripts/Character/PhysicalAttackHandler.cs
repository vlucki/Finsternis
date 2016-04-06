﻿using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class PhysicalAttackHandler : MonoBehaviour
{
    public Character owner;
    public List<Collider> ignoreList;
    public Vector2 impactMultiplier = new Vector2(2, 3);
    public ForceMode modeOnImpact = ForceMode.VelocityChange;
    public Vector2 executionMultiplier = new Vector2(10, 2);
    public ForceMode modeOnExecution = ForceMode.Impulse;

    void Start()
    {
        if (!owner)
        {
            owner = GetComponentInParent<Character>();
            if (!owner)
                throw new System.InvalidOperationException("Attack handler needs an owner!");
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (ignoreList != null)
            if (ignoreList.Contains(other))
                return;

        if (!owner.GetComponent<Animator>().GetBool("attacking"))
            return;

        Rigidbody body = other.GetComponent<Rigidbody>();
        if (body)
        {
            Vector3 dir = (other.transform.position - owner.transform.position);
            body.AddForce(dir * impactMultiplier.x + Vector3.up * impactMultiplier.y, modeOnImpact);
            Vector3 target = owner.transform.position;
            target.y = other.transform.position.y;
            other.transform.LookAt(target);
            Character otherChar = other.GetComponent<Character>();

            Animator a = other.GetComponent<Animator>();
            if (a){
                a.SetBool("beingHit", true);
            }
            if (otherChar)
            {
                RangedValueAttribute health = otherChar.health;
                
                (health).SetValue(health.Value - GetComponentInParent<Character>().damage.Value);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!owner.GetComponent<Animator>().GetBool("attacking"))
            return;

        if (ignoreList != null)
            if (ignoreList.Contains(other))
                return;

        Rigidbody body = other.GetComponent<Rigidbody>();
        if (body)
        {
            body.AddForce((other.transform.position - owner.transform.position) * executionMultiplier.x + Vector3.up * executionMultiplier.y, modeOnExecution);
        }
    }
}