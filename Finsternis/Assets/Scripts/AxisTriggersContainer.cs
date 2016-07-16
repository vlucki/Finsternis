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
    private string _axis;

    [SerializeField]
    private ThresholdTypeEnum _thresholdType = ThresholdTypeEnum.DIFFERENT_THAN;

    [Range(-1, 1)]
    [SerializeField]
    private float thresholdValue = 0;

    [Range(0, 10)]
    [SerializeField]
    private float _repeatDelay = 0f;

    private float _lastTriggered;

    public AxisInputEvent onAxisActive;

    public string Axis
    {
        get { return _axis; }
        set { _axis = value; }
    }

    public ThresholdTypeEnum ThresholdType
    {
        get { return _thresholdType; }
        set { _thresholdType = value; }
    }

    public float ThresholdValue
    {
        get { return thresholdValue; }
        set { thresholdValue = value; }
    }

    public float LastTriggered
    {
        get { return _lastTriggered; }
        set { _lastTriggered = value; }
    }

    public float RepeatDelay
    {
        get { return _repeatDelay; }
        set { _repeatDelay = value; }

    }
}