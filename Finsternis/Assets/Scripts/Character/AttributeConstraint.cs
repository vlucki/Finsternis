using UnityEngine;
using System.Collections;

public abstract class AttributeConstraint
{
    protected EntityAttribute attribute;

    public EntityAttribute Attribute
    {
        get { return attribute; }
    }

    public AttributeConstraint() { }

    public AttributeConstraint(EntityAttribute attribute)
    {
        this.attribute = attribute;
    }

    public virtual void SetAttribute(EntityAttribute attribute)
    {
        this.attribute = attribute;
    }

    public abstract bool Check();

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        if (!obj.GetType().Equals(this.GetType()))
            return false;

        return true;
    }
}
