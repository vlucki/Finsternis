using UnityEngine;

[RequireComponent(typeof(Entity))]
public abstract class EntityAction : MonoBehaviour
{
    protected Entity agent;

    protected virtual void Awake()
    {
        agent = GetComponent<Entity>();
    }

    public abstract void Perform(params object[] parameters);
}
