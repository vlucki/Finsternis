using System;
using UnityEngine;

[RequireComponent(typeof(Movement), typeof(Animator))]
public class PlayerController : CharacterController
{
    private Vector3 _directionFaced;

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
            characterMovement.Direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            _directionFaced = GetDirectionFaced();
            transform.LookAt(_directionFaced);

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
            dir = characterMovement.Direction;
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
                    Entity characterHit = col.GetComponent<Entity>();
                    if (characterHit)
                    {
                        characterHit.GetComponent<Rigidbody>().AddExplosionForce(200, transform.position, 20, 5, ForceMode.Impulse);
                        characterHit.GetComponent<CharacterController>().Hit();
                        this.character.DoDamage(characterHit);
                    }
                }
            }
        }
        base.Attack(type, lockMovement);
    }
}
