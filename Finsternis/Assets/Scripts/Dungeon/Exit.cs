﻿using UnityEngine;
using System.Collections.Generic;
using MovementEffects;
using UnityEngine.Events;
using System;

public class Exit : Trigger
{
    //[SerializeField]
    //private BoxCollider _trigger;

    [Serializable]
    public class ExitCrossedEvent : UnityEvent<Exit>
    {
        public static implicit operator bool(ExitCrossedEvent evt)
        {
            return evt != null;
        }
    }

    [SerializeField]
    private GameObject _player;

    [SerializeField]
    private GameObject _cameraHolder;

    private bool _locked;

    private bool _triggered;

    public ExitCrossedEvent OnExitCrossed;

    public bool Locked { get { return _locked; } }

    protected override void Awake()
    {
        base.Awake();
        if (!OnExitCrossed)
            OnExitCrossed = new ExitCrossedEvent();
        OnExitCrossed.AddListener(GameManager.Instance.EndCurrentLevel);
        _player = GameObject.FindGameObjectWithTag("Player");
        _cameraHolder = GameObject.FindGameObjectWithTag("MainCamera").transform.parent.gameObject;
        _locked = true;

        _triggered = false;
    }

    public void Unlock()
    {
        if (!collider.enabled)
        {
            collider.enabled = true;
            Follow camFollow = _cameraHolder.GetComponent<Follow>();
            camFollow.SetTarget(transform);
            camFollow.OnTargetReached.AddListener(BeginOpen);
        }
    }

    private void BeginOpen()
    {
        GetComponent<Animator>().SetTrigger("Open");
    }

    public void Open()
    {
        Follow camFollow = _cameraHolder.GetComponent<Follow>();
        camFollow.OnTargetReached.RemoveListener(BeginOpen);
        camFollow.SetTarget(_player.transform);
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        if (ObjectExited == _player)
        {
            if (other.transform.position.y < transform.position.y)
            {
                if(OnExitCrossed)
                    OnExitCrossed.Invoke(this);
            }
        }
    }
}
