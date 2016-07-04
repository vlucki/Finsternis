using UnityEngine;
using System.Collections.Generic;
using MovementEffects;

[RequireComponent(typeof(AttackAction))]
public class TrapBehaviour : Entity
{
    public float damageModifierOnTouch = 2;
    public float damageModifierOnStay = 1;

    public HashSet<Entity> entitiesInContact;

    protected Dungeon dungeon;

    protected Vector2 coordinates = -Vector2.one;

    protected AttackAction attack;

    protected void Awake()
    {
        entitiesInContact = new HashSet<Entity>();
        if (!attack)
            attack = GetComponent<AttackAction>();
    }

    public void Init(Vector2 coordinates)
    {
        if (this.coordinates == -Vector2.one)
            this.coordinates = coordinates;
        Align();
    }

    protected virtual void Align()
    {
        dungeon = GameObject.FindGameObjectWithTag("Dungeon").GetComponent<Dungeon>();
        try
        {
            Corridor corridor = dungeon.Corridors.Find(c => c.Bounds.Contains(coordinates));
            Vector2 corridorDir = corridor.Direction;
            transform.forward = new Vector3(corridorDir.y, 0, corridorDir.x);
        }
        catch (System.NullReferenceException ex)
        {
            Debug.LogError("Failed to find corridor containing the coordinate " + coordinates);
            Debug.LogError(ex.Message);
            DestroyImmediate(gameObject);
        }
    }

    public virtual IEnumerator<float> _OnContinuousTouch(Entity e)
    {
        yield return 0f;
        while (entitiesInContact.Contains(e))
        {
            attack.Perform(e, DamageInfo.DamageType.physical, damageModifierOnStay);
            yield return 0f;
        }
    }

    public virtual void OnTouch(GameObject touched)
    {
        Entity e = touched.GetComponentInParent<Entity>();
        if (e)
        {
            if (!entitiesInContact.Contains(e))
            {
                attack.Perform(e, DamageInfo.DamageType.physical, damageModifierOnTouch);
                entitiesInContact.Add(e);
                Timing.RunCoroutine(_OnContinuousTouch(e));
            }
        }
    }

    public virtual void OnExit(GameObject exited)
    {
        Entity e = exited.GetComponentInParent<Entity>();
        if (e)
        {
            entitiesInContact.Remove(e);
        }
    }
}
