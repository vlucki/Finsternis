using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class AttributeConstraint
{
    [SerializeField]
    protected EntityAttribute attribute;

    [SerializeField]
    private string _name;

    public string Name { get { return _name; } }

    public EntityAttribute Attribute
    {
        get { return attribute; }
    }

    public AttributeConstraint(string name = null) { this._name = name; }

    public AttributeConstraint(EntityAttribute attribute, string name = null) : this(name)
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
        return attribute.GetHashCode() * (!System.String.IsNullOrEmpty(_name) ? 97 : _name.GetHashCode());
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
