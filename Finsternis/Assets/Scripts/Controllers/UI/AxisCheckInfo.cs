using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AxisCheckInfo
{
    [System.Serializable]
    public class AxisInputEvent : UnityEvent<float>
    {
        public static implicit operator bool(AxisInputEvent evt)
        {
            return evt != null;
        }
    }

    [AxesName]
    public string _axis;

    public enum ThresholdType
    {
        ANY = 0,
        LESS_THAN = 1,
        GREATER_THAN = 2,
        DIFFERENT_THAN = 3,
        EQUAL_TO = 4
    }

    public ThresholdType thresholdType = ThresholdType.DIFFERENT_THAN;

    [Range(-1, 1)]
    public float thresholdValue = 0;

    public string Axis { get { return _axis; } }

    public AxisInputEvent onAxisActive;
}