using UnityEngine;

public class PlayerMovement : Movement
{

    [SerializeField]
    private GameObject characterModel;

    private Animator characterAnimator;

    protected override void Start()
    {
        base.Start();
        rigidBody.freezeRotation = true;
        characterAnimator = characterModel.GetComponent<Animator>();
    }

    void Update()
    {
        Direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        bool walking = Direction != Vector3.zero;

        characterAnimator.SetBool("walking", walking);
        if (walking)
            characterModel.transform.LookAt(characterModel.transform.position + Direction);
    }

}
