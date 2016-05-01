using System;
using UnityEngine;

[RequireComponent(typeof(Movement), typeof(Animator))]
public class PlayerController : CharacterController
{


    [SerializeField]
    [Tooltip("Wheter the usual WASD should respond according to the direction faced by the character.")]
    private bool _adjustControls = true;

    [Range(0, 1)]
    private float _turningSpeed = 0.05f;

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
            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (_adjustControls)
            {
                movement = transform.right * movement.x + transform.forward * movement.z;
            }

            characterMovement.Direction = movement;

            transform.forward = Vector3.Slerp(transform.forward, characterMovement.Direction, _turningSpeed);

        }
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
