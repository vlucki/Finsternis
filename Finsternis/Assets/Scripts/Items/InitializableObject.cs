public abstract class InitializableObject : UnityEngine.ScriptableObject
{
    public bool Initialized { get; private set; }

    protected virtual void Init()
    {
        Initialized = true;
    }

}