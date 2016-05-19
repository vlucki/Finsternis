using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(AttackAction))]
public class Trap : Entity
{
    public float damageModifierOnTouch = 2;
    public float damageModifierOnStay = 1;

    public HashSet<Entity> entitiesInContact;

    protected SimpleDungeon dungeon;

    protected Vector2 coordinates = -Vector2.one;

    protected AttackAction attack;

    protected override void Awake()
    {
        base.Awake();
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
        dungeon = GameObject.FindGameObjectWithTag("Dungeon").GetComponent<SimpleDungeon>();
        try
        {
            Corridor corridor = dungeon.Corridors.Find(c => c.Bounds.Contains(coordinates));
            Vector2 corridorDir = corridor.Direction;
            transform.forward = new Vector3(corridorDir.y, 0, corridorDir.x);
        }
        catch (System.NullReferenceException ex)
        {
            Debug.LogError("Failed to find corridor containing the coordinate " + coordinates);
            GameObject.DestroyImmediate(this.gameObject);
        }
    }

    public virtual IEnumerator OnContinuousTouch(Entity e)
    {
        yield return new WaitForEndOfFrame();
        while (entitiesInContact.Contains(e))
        {
            attack.Perform(e, DamageInfo.DamageType.physical, damageModifierOnStay);
            yield return new WaitForEndOfFrame();
        }
    }

    public virtual void OnTouch(Trigger source)
    {
        Entity e = source.ObjectEntered.GetComponentInParent<Entity>();
        if (e)
        {
            if (!entitiesInContact.Contains(e))
            {
                attack.Perform(e, DamageInfo.DamageType.physical, damageModifierOnTouch);
                entitiesInContact.Add(e);
                StartCoroutine(OnContinuousTouch(e));
            }
        }
    }

    public virtual void OnExit(Trigger source)
    {
        Entity e = source.ObjectExited.GetComponentInParent<Entity>();
        if (e)
        {
            entitiesInContact.Remove(e);
        }
    }
}
