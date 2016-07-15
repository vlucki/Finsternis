using UnityEngine;
using UnityQuery;

public class InputAxisChecker : MonoBehaviour
{
    [SerializeField]
    private AxisCheckInfo[] _axesToCheck;
    
    void Update()
    {
        if (_axesToCheck != null && _axesToCheck.Length > 0)
        {
            for (int i = 0; i < _axesToCheck.Length; i++)
            {
                AxisCheckInfo axis = _axesToCheck[i];
                float value = Input.GetAxis(axis.Axis);
                if (ShouldTrigger(axis, value))
                    if (axis.onAxisActive)
                        axis.onAxisActive.Invoke(value);
                    else
                        Log.Warn("No method assigned to axis " + axis.Axis);
            }
        }
    }

    private bool ShouldTrigger(AxisCheckInfo axis, float value)
    {
        switch (axis.thresholdType)
        {
            case AxisCheckInfo.ThresholdType.DIFFERENT_THAN:
                return value != axis.thresholdValue;
            case AxisCheckInfo.ThresholdType.LESS_THAN:
                return value < axis.thresholdValue;
            case AxisCheckInfo.ThresholdType.GREATER_THAN:
                return value > axis.thresholdValue;
            case AxisCheckInfo.ThresholdType.EQUAL_TO:
                return value == axis.thresholdValue;
            default:
                return false;
        }
    }
}