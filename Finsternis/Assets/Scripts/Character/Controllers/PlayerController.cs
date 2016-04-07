using System;
using UnityEngine;

[RequireComponent(typeof(Movement), typeof(Animator))]
public class PlayerController : CharacterController
{
    private Vector3 facingDirection;

    public override void Update()
    {
        base.Update();
        if (Input.GetAxis("Jump") != 0 && !IsAttacking())
        {
            Attack();
        }

        if (!IsAttacking())
        {
            _characterMovement.Direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            facingDirection = GetDirectionFaced();
            transform.LookAt(facingDirection);

        }
    }

    private Vector3 GetDirectionFaced()
    {
        Vector3 dir = Vector3.zero;
        if (Input.GetKey(KeyCode.LeftArrow))
            dir.x--;
        if (Input.GetKey(KeyCode.RightArrow))
            dir.x++;
        if (Input.GetKey(KeyCode.UpArrow))
            dir.z++;
        if (Input.GetKey(KeyCode.DownArrow))
            dir.z--;
        if (dir == Vector3.zero)
            dir = _characterMovement.Direction;
        return transform.position + dir;
    }

    public override void Attack(int type = 0, bool lockMovement = true)
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            type = 1;
            Collider[] c = Physics.OverlapSphere(transform.position, 3);
            foreach (Collider col in c)
            {
                if (col.gameObject.Equals(gameObject))
                    continue;
                else
                {
                    Character character = col.GetComponent<Character>();
                    if (character)
                    {
                        col.GetComponent<Rigidbody>().AddExplosionForce(200, transform.position, 20, 5, ForceMode.Impulse);
                        col.GetComponent<CharacterController>().Hit();

                        RangedValueAttribute health = character.health;

                        (health).SetValue(health.Value - GetComponentInParent<Character>().damage.Value);
                    }
                }
            }
        }
        base.Attack(type, lockMovement);
    }

    protected override void CharacterController_death()
    {
        base.CharacterController_death();
    }
}
