using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class InputRouter : MonoBehaviour
{
    [System.Serializable]
    public class InputTrigger
    {
        [System.Serializable]
        public class AxisInputEvent : UnityEvent<float>
        {
            public static implicit operator bool(AxisInputEvent evt)
            {
                return evt != null;
            }
        }

        public string name;

        public InputControl[] controls;

        public AxisInputEvent onAxisActive;

        private float lastTriggered;

        public void Trigger()
        {
            System.Array.ForEach(controls, (control) =>
            {
                if (ShouldTrigger(control))
                {
                    lastTriggered = Time.timeSinceLevelLoad;
                    onAxisActive.Invoke(control.Value());
                }
            });
        }

        private bool ShouldTrigger(InputControl control)
        {
            if (!control)
            {
                UnityQuery.Log.Warn(control, "Null control found. Did you forget to set something in the inspector?");
                return false;
            }
            if (!control.Enabled || Time.timeSinceLevelLoad - lastTriggered < control.RepeatDelay)
                return false;

            float value = control.TrueValue();

            switch (control.ThresholdType)
            {
                case InputControl.
                    ThresholdTypeEnum.DIFFERENT_THAN:
                    return value != control.ThresholdValue;

                case InputControl.
                    ThresholdTypeEnum.LESS_THAN:
                    return value < control.ThresholdValue;

                case InputControl.
                    ThresholdTypeEnum.GREATER_THAN:
                    return value > control.ThresholdValue;

                case InputControl.
                    ThresholdTypeEnum.EQUAL_TO:
                    return value == control.ThresholdValue;

                case InputControl.
                    ThresholdTypeEnum.ANY:
                    return true;

                default:
                    return false;
            }
        }

        internal void SetControlState(string axis, bool enabled)
        {
            System.Array.ForEach(controls, (control) => { if (control.Axis.Equals(axis)) control.Enabled = enabled; });
        }
    }

    [SerializeField]
    private InputTrigger[] triggers;

    void Update()
    {
        if (this.triggers != null && this.triggers.Length > 0)
        {
            foreach(var trigger in this.triggers) trigger.Trigger();
        }
    }

    public void Disable(string axis)
    {
        foreach (var trigger in this.triggers) trigger.SetControlState(axis, false);
    }

    public void Enable(string axis)
    {
        foreach (var trigger in this.triggers) trigger.SetControlState(axis, true);
    }
}