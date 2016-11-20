using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityQuery;

public class CustomBehaviour : MonoBehaviour
{
    [HideInInspector]
    public new Transform transform;

    protected virtual void Awake()
    {
        this.transform = base.transform;
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }    
}
