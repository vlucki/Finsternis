using System;
using UnityEngine;

[RequireComponent(typeof(Movement), typeof(Animator))]
public class PlayerController : MonoBehaviour
{ 
    private Animator _characterAnimator;
    private Movement _characterMovement;

    private Vector3 facingDirection;

    void Start()
    {
        _characterMovement = GetComponent<Movement>();
        _characterAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetAxis("Jump") != 0)
        {
            _characterAnimator.SetBool("attacking", true);
            _characterMovement.Direction = Vector3.zero;
        }

        if (!_characterAnimator.GetBool("attacking"))
        {
            _characterMovement.Direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            bool walking = _characterMovement.Direction != Vector3.zero;

            _characterAnimator.SetBool("walking", walking);
            if (walking && !Input.GetKey(KeyCode.LeftShift))
            {
                facingDirection = GetDirectionFaced();
                transform.LookAt(facingDirection);
            }
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
}
