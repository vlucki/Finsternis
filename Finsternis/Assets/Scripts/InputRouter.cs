using UnityEngine;
using UnityQuery;

[DisallowMultipleComponent]
public class InputRouter : MonoBehaviour
{
    [SerializeField]
    private AxisTriggersContainer[] axesToCheck;

    void Update()
    {
        if (this.axesToCheck != null && this.axesToCheck.Length > 0)
        {
            for (int i = 0; i < this.axesToCheck.Length; i++)
            {
                AxisTriggersContainer axis = this.axesToCheck[i];
                float value = Input.GetAxis(axis.Axis);
                if (ShouldTrigger(axis, value))
                    if (axis.onAxisActive)
                        axis.onAxisActive.Invoke(value);
                    else
                        Log.Warn("No method assigned to axis " + axis.Axis);
            }
        }
    }

    private bool ShouldTrigger(AxisTriggersContainer axis, float value)
    {
        if (!axis.Enabled || Time.timeSinceLevelLoad - axis.LastTriggered < axis.RepeatDelay)
            return false;

        axis.LastTriggered = Time.timeSinceLevelLoad;

        switch (axis.ThresholdType)
        {
            case AxisTriggersContainer.
                ThresholdTypeEnum.DIFFERENT_THAN:
                return value != axis.ThresholdValue;

            case AxisTriggersContainer.
                ThresholdTypeEnum.LESS_THAN:
                return value < axis.ThresholdValue;

            case AxisTriggersContainer.
                ThresholdTypeEnum.GREATER_THAN:
                return value > axis.ThresholdValue;

            case AxisTriggersContainer.
                ThresholdTypeEnum.EQUAL_TO:
                return value == axis.ThresholdValue;

            case AxisTriggersContainer.
                ThresholdTypeEnum.ANY:
                return true;

            default:
                return false;
        }
    }
}