using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public sealed class AxisTriggersContainer
{
    [System.Serializable]
    public class AxisInputEvent : UnityEvent<float>
    {
        public static implicit operator bool(AxisInputEvent evt)
        {
            return evt != null;
        }
    }

    public enum ThresholdTypeEnum
    {
        NONE            = 0x000,
        LESS_THAN       = 0x001,
        GREATER_THAN    = 0x010,
        DIFFERENT_THAN  = 0x011,
        EQUAL_TO        = 0x100,
        ANY             = 0x111
    }

#if UNITY_EDITOR
    public string name;
#endif

    [AxesName][SerializeField]
    private string axis;

    [SerializeField]
    private bool enabled = true;

    [SerializeField]
    private ThresholdTypeEnum thresholdType = ThresholdTypeEnum.DIFFERENT_THAN;

    [Range(-1, 1)]
    [SerializeField]
    private float thresholdValue = 0;

    [Range(0, 10)]
    [SerializeField]
    private float repeatDelay = 0f;

    private float lastTriggered;

    public AxisInputEvent onAxisActive;

    public string Axis
    {
        get { return this.axis; }
        set { this.axis = value; }
    }

    public bool Enabled
    {
        get { return this.enabled; }
        set { this.enabled = value; }
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

    public float LastTriggered
    {
        get { return lastTriggered; }
        set { lastTriggered = value; }
    }

    public float RepeatDelay
    {
        get { return this.repeatDelay; }
        set { this.repeatDelay = value; }

    }
}