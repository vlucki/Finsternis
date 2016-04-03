using System;
using UnityEngine;

public class FloatValueConstraint : AttributeConstraint
{
    private const byte NONE = 0; //0000
    public const byte LESS_THAN = 1; //0001
    public const byte GREATER_THAN = 2; //0010
    public const byte NOT_EQUALS = LESS_THAN | GREATER_THAN; //3 or 0011 
    public const byte EQUALS = (NOT_EQUALS) << 2; //12 or 1100


    private byte _comparisonType;
    private float _value;

    public float Value { get { return _value; } }


    public FloatValueConstraint(float value, byte comparisonType, string name = null) : base(name)
    {
        string msg;
        if (!ValidateComparison(comparisonType, out msg))
        {
            throw new System.ArgumentException(msg);
        }

        _value = value;
        _comparisonType = comparisonType;
    }

    public FloatValueConstraint(float value, string comparisonString, string name = null) : this(value, ParseComparisonString(comparisonString), name) { }

    private static byte ParseComparisonString(string comparisonString)
    {
        byte type = NONE;

        if (comparisonString.Length < 3)
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

    private bool ValidateComparison(byte comparisonType, out string message)
    {
        message = null;
        if ((comparisonType | EQUALS) == NONE
         && (comparisonType | NOT_EQUALS) == NONE) //if it is either < or >, it would not result in "NONE"
        {
            message = "Invalid comparison type: " + comparisonType;
            return false;
        }
        else
        {
            if ((comparisonType & NOT_EQUALS) == NOT_EQUALS 
             && (comparisonType ^ NOT_EQUALS) != NONE)
            {
                message = "Not equals cannot be used with equals.";
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

        //if attributeValue is equal to _value when it should be different
        if ((_comparisonType & NOT_EQUALS) == NOT_EQUALS && attributeValue == _value)
            return false;

        //if attributeValue is not equal to _value when it should be
        if ((_comparisonType ^ EQUALS) == NONE && attributeValue != _value)
            return false;

        byte comparison = (byte)(_comparisonType ^ LESS_THAN);
        
        if ((comparison == NONE && attributeValue >= _value) || //if attributeValue must be less than value
            (comparison == EQUALS && attributeValue > _value))  //or less than or equal to _value, but isn't
            return false;

        comparison = (byte)(_comparisonType ^ GREATER_THAN);

        if ((comparison == NONE && attributeValue <= _value) || //if attributeValue must be greater than value
            (comparison == EQUALS && attributeValue < _value))  //or greater than or equal to _value, but isn't
            return false;


        return true;
    }

    public override int GetHashCode()
    {
        return (int)(base.GetHashCode() + _comparisonType * 17 + _value * (_value + 97));
    }

    public override bool Equals(object obj)
    {
        if (base.Equals(obj))
        {
            FloatValueConstraint other = (FloatValueConstraint)obj;
            if (other.attribute == this.attribute && other._comparisonType == this._comparisonType && other._value == this._value)
            {
                return true;
            }
        }

        return false;
    }

    public override string ToString()
    {
        return base.ToString() + " (comparison type: " + _comparisonType + "; value: " + _value + ")";
    }
}
