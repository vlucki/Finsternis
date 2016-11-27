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

    private Dictionary<Type, Component> componentCache;

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
        foreach (var behaviour in this.GetComponents<MonoBehaviour>())
        {
            Destroy(behaviour);
        }
    }

    public T CacheComponent<T>() where T : Component
    {
        if (CacheComponent<T>(GetComponent<T>()))
            return GetCachedComponent<T>(false);

        return null;
    }

    public bool CacheComponent<T>(T component) where T : Component
    {
        if (component == null)
            return false;

        if (this.componentCache == null)
            this.componentCache = new Dictionary<Type, Component>();
        this.componentCache[typeof(T)] = component;
        return true;
    }

    public T GetCachedComponent<T>(bool cacheIfNotFound = true) where T : Component
    {
        Component component = null;
        if (this.componentCache != null)
        {
            Type t = typeof(T);
            this.componentCache.TryGetValue(t, out component);
        }
        if (!component && cacheIfNotFound)
        {
            component = GetComponent<T>();
            if (!CacheComponent<T>(component as T))
                return null;
        }

        return component as T;
    }
}
