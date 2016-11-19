using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InputControl", menuName = "Finsternis/Input/Input", order = 1)]
public class InputControl : ScriptableObject
{
    public enum ThresholdTypeEnum
    {
        NONE            = 0x000,
        LESS_THAN       = 0x001,
        GREATER_THAN    = 0x010,
        DIFFERENT_THAN  = 0x011,
        EQUAL_TO        = 0x100,
        ANY             = 0x111
    }

    [AxesName][SerializeField]
    private string axis;

    [SerializeField]
    private ThresholdTypeEnum thresholdType = ThresholdTypeEnum.DIFFERENT_THAN;

    [Range(-1, 1)]
    [SerializeField]
    private float thresholdValue = 0;

    [Range(0, 10)]
    [SerializeField]
    private float repeatDelay = 0f;

    [SerializeField]
    private bool enabled = true;

    public string Axis
    {
        get { return this.axis; }
        set { this.axis = value; }
    }

    public ThresholdTypeEnum ThresholdType
    {
        get { return this.thresholdType; }
        set { this.thresholdType = value; }
    }

    public float ThresholdValue
    {
        get { return this.thresholdValue; }
        set { this.thresholdValue = value; }
    }

    public float RepeatDelay
    {
        get { return this.repeatDelay; }
        set { this.repeatDelay = value; }
    }

    /// <summary>
    /// Value of this control. By default, it will be the same as the AxisValue.
    /// </summary>
    public virtual float Value
    {
        get { return AxisValue; }
    }

    /// <summary>
    /// Unmodified value from the axis.
    /// </summary>
    public float AxisValue {
        get {
            float axisValue = Input.GetAxis(Axis);
            return axisValue;
        } }

    /// <summary>
    /// Returns false if the axis value is 0, true otherwise.
    /// </summary>
    public bool BooleanValue { get { return Value != 0; } }

    /// <summary>
    /// Returns 0 if the axis value is 0, -1 if it is less than 0 and 1 otherwise
    /// </summary>
    public int ConstantValue
    {
        get
        {
            float rawValue = Value;
            if (rawValue < 0)
                return -1;
            if (rawValue > 0)
                return 1;
            return 0;
        }
    }

    public bool IsEnabled() { return this.enabled; }

    public void Enable() { this.enabled = true; }

    public void Disable() { this.enabled = false; }

    public void Toggle() { this.enabled = !this.enabled; }

#if UNITY_EDITOR

    void OnValidate()
    {
        this.ThresholdValue = Mathf.Clamp(this.thresholdValue, -1, 1);
    }
#endif
}