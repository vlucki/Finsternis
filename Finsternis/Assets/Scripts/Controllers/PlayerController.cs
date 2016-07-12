using UnityEngine;

namespace Finsternis
{
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
            base.Attack(type, lockMovement);
        }
    }
}