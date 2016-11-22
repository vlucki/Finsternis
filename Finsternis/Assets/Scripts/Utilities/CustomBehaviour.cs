using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityQuery;

public class CustomBehaviour : MonoBehaviour
{
    [Serializable]
    public enum DeactivateMethod { NONE = 0, AWAKE = 1, START = 2, ENABLE = 3 }
    [HideInInspector]
    public new Transform transform;

    [SerializeField]
    private DeactivateMethod deactivate = DeactivateMethod.NONE;

    protected virtual void Awake()
    {
        this.transform = base.transform;
        if (this.deactivate == DeactivateMethod.AWAKE)
            this.gameObject.SetActive(false);
    }

    protected virtual void Start()
    {
        if (this.deactivate == DeactivateMethod.START)
            this.gameObject.SetActive(false);
    }

    protected virtual void OnEnable()
    {
        if (this.deactivate == DeactivateMethod.ENABLE)
            this.gameObject.SetActive(false);
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    public void Remove()
    {
        Destroy(this);
    }

    public void RemoveAllBehaviours()
    {
        foreach(var behaviour in this.GetComponents<MonoBehaviour>())
        {
            Destroy(behaviour);
        }
    }
}
