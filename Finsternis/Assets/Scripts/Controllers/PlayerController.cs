using System;
using UnityEngine;

[RequireComponent(typeof(Movement), typeof(Animator))]
public class PlayerController : CharacterController
{
    public override void Update()
    {
        base.Update();

        if (Locked || IsDead() || IsDying())
            return;

        if (!IsAttacking())
        {
            CheckAttack();
        }

        if (!IsAttacking())
        {
            Move(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
        }
    }

    private void CheckAttack()
    {
        if (Input.GetAxis("Jump") > 0)
            Attack();
        else if (Input.GetAxis("Fire1") > 0)
            Attack(1);
        else if (Input.GetAxis("Fire2") > 0)
            Attack(2);
        else if (Input.GetAxis("Fire3") > 0)
            Attack(3);

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
