﻿using UnityEngine;
using System.Collections;

public class DeathAnimationController : StateMachineBehaviour {
    [SerializeField]
    [Range(0, 10)]
    private float _delayAfterFinish = 0.5f;
    private float _elapsedTime;
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_elapsedTime < stateInfo.length + _delayAfterFinish)
        {
            _elapsedTime += Time.deltaTime;
        }
        else if(!animator.GetBool(CharacterController.DeadBool))
        {
            animator.SetBool(CharacterController.DyingBool, false);
            animator.SetBool(CharacterController.DeadBool, true);
        }
    }
}
