﻿using System;
using UnityEngine;

[RequireComponent(typeof(Movement), typeof(Animator))]
public class PlayerController : CharacterController
{

    public override void Awake()
    {
        base.Awake();
    }

    public override void Update()
    {
        base.Update();

        if (Locked || IsDead() || IsDying())
        {
            if(IsDead())
                GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().GameOver();
            return;
        }

        if (Input.GetAxis("Jump") != 0 && !IsAttacking())
        {
            Attack();
        }

        if (!IsAttacking())
        {
            Move(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
        }
    }

    public override void Attack(int type = 0, bool lockMovement = true)
    {
        //AoE attack test
        //if (Input.GetKey(KeyCode.LeftShift))
        //{
        //    type = 1;
        //    Collider[] c = Physics.OverlapSphere(transform.position, 3);
        //    foreach (Collider col in c)
        //    {
        //        if (col.gameObject.Equals(gameObject))
        //            continue;
        //        else
        //        {
        //            Entity characterHit = col.GetComponent<Entity>();
        //            if (characterHit)
        //            {
        //                CharacterController controller = characterHit.GetComponent<CharacterController>();
        //                if (controller)
        //                {
        //                    characterHit.GetComponent<Rigidbody>().AddExplosionForce(200, transform.position, 20, 5, ForceMode.Impulse);
        //                    controller.Hit();
        //                }
        //                this.character.GetComponent<AttackAction>().Perform(characterHit);
        //            }
        //        }
        //    }
        //}
        base.Attack(type, lockMovement);
    }
}
