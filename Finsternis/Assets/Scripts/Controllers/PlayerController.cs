using UnityEngine;

namespace Finsternis
{
    [RequireComponent(typeof(Movement), typeof(Animator))]
    public class PlayerController : CharacterController
    {
        //public override void Update()
        //{
        //    base.Update();

        //    if (Locked || IsDead() || IsDying())
        //        return;

        //    SetXDirection(Input.GetAxis("Horizontal"));
        //    SetZDirection(Input.GetAxis("Vertical"));
        //    //SetDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));

        //}
    }
}