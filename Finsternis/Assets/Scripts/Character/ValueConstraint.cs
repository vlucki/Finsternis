using System;
using UnityEngine;

public class ValueConstraint : AttributeConstraint
{
    private const byte NONE         = 0; //0000
    public const byte LESS_THAN     = 1; //0001
    public const byte GREATER_THAN  = 2; //0010
    public const byte NOT_EQUALS    = LESS_THAN | GREATER_THAN; //0011 
    public const byte EQUALS        = (NOT_EQUALS) << 2; //1100


    private int _comparisonType;
    private float _value;

    public override void SetAttribute(EntityAttribute value)
    {
        Type t = value.GetType();
        if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(EntityAttribute<>) && t.GetGenericArguments()[0] == typeof(float))
        {
            base.SetAttribute(value);
        }
        else
            throw new ArgumentException("Value constraint must have an attribute of type 'float'.");
        
    }

    public ValueConstraint(float value, byte comparisonType)
    {
        string msg;
        if (!ValidateComparison(comparisonType, out msg))
        {
            throw new System.ArgumentException(msg);
        }

        _value = value;
        _comparisonType = comparisonType;
    }

    public ValueConstraint(float value, string comparisonString) : this(value, ParseComparisonString(comparisonString))
    {
    }

    private static byte ParseComparisonString(string comparisonString)
    {
        byte type = NONE;

        if(comparisonString.Length < 3)
        {
            foreach (char c in comparisonString)
            {
                if (c == '<')
                    type |= LESS_THAN;
                else if (c == '>')
                    type |= GREATER_THAN;
                else if (c == '=')
                    type |= EQUALS;
            }
        }


        return type;
    }

    private bool ValidateComparison(int comparisonType, out string message)
    {
        message = null;
        if ((comparisonType | EQUALS) == NONE
            && (comparisonType | NOT_EQUALS) == NONE)
        {
            message = "Invalid comparison type: " + comparisonType;
            return false;
        }
        else
        {
            if ((comparisonType & NOT_EQUALS) == NOT_EQUALS && (comparisonType ^ NOT_EQUALS) != NONE)
            {
                message = "Inequallity should not be used in conjunct with other comparison types.";
                return false;
            }
        }

        return true;
    }

    //TODO: fix this mess up...
    public override bool Check()
    {
        float attributeValue = ((EntityAttribute<float>)attribute).Value;

        //if the attribute value is....

        //equal to the specified value when it should be different
        if ((_comparisonType & NOT_EQUALS) == NOT_EQUALS && attributeValue == _value)
            return false;

        //greater than the specified value when it should be smaller
        if ((_comparisonType ^ LESS_THAN) == NONE && attributeValue >= _value)
            return false;

        //smaller than the specified value when it should be greater
        if ((_comparisonType ^ GREATER_THAN) == NONE && attributeValue <= _value)
            return false;

        //different than the specified value when it should only be equal
        if ((_comparisonType ^ EQUALS) == NONE && attributeValue != _value)
            return false;

        if ((_comparisonType ^ (LESS_THAN | EQUALS)) == NONE && attributeValue >= _value)
            return false;

        if ((_comparisonType ^ (GREATER_THAN | EQUALS)) == NONE && attributeValue <= _value)
            return false;


        return true;
    }

    public override bool Equals(object obj)
    {
        if (base.Equals(obj))
        {
            ValueConstraint other = (ValueConstraint)obj;
            if (other.attribute == this.attribute && other._comparisonType == this._comparisonType && other._value == this._value)
            {
                return true;
            }
        }

        return false;
    }
}
