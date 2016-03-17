using UnityEngine;

public class PlayerMovement : Movement
{

    protected override void Start()
    {
        base.Start();
        body.freezeRotation = true;
    }

    void Update()
    {
        Direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }

}
