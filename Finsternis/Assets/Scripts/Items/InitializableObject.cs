﻿public abstract class InitializableObject : UnityEngine.ScriptableObject
{
    public bool Initialized { get; private set; }

    protected void Init()
    {
        Initialized = true;
    }

    public void InitCheck()
    {
        if (!Initialized)
            throw new System.InvalidOperationException("Object was not initialized.");
    }

}