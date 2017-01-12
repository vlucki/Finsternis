using UnityEngine;

public class AxesNameAttribute : PropertyAttribute
{
    public readonly bool allowNone;
    public AxesNameAttribute(bool allowNone = false)
    {
        this.allowNone = allowNone;
    }
}