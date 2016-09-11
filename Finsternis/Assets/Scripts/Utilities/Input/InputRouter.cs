using UnityEngine;
using UnityEngine.Events;
using UnityQuery;

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

        public bool enabled = true;

        [Tooltip("If true, will wait for the input to stop firing before triggering again.")]
        public bool toggleOnly = false;

        public InputControl[] controls;

        public AxisInputEvent onAxisActive;

        private bool active;

        private float lastTriggered;

        public void Trigger()
        {
            System.Array.ForEach(controls, (control) =>
            {
                if (ShouldTrigger(control))
                {
                    lastTriggered = Time.timeSinceLevelLoad;
                    onAxisActive.Invoke(control.Value);
                    active = true;
                }
                else if (!control.BooleanValue)
                {
                    active = false;
                }
            });
        }

        private bool ShouldTrigger(InputControl control)
        {
            if (!control)
            {
                Log.Warn(control, "Null control found. Did you forget to set something in the inspector?");
                return false;
            }
            if (!control.IsEnabled() || (active && toggleOnly) || (active && Time.timeSinceLevelLoad - lastTriggered < control.RepeatDelay))
                return false;

            float value = control.AxisValue;

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
            System.Array.ForEach(controls, (control) => {
                if (control.Axis.Equals(axis))
                {
                    if (!(enabled && control.IsEnabled())) control.Toggle(); //if the control stat does not match the desired one, toggle it
                }
            });
        }
    }

    [SerializeField]
    private InputTrigger[] triggers;

    void Update()
    {
        if (this.triggers != null && this.triggers.Length > 0)
        {
            foreach(var trigger in this.triggers) if(trigger.enabled) trigger.Trigger();
        }
    }

    public void DisableTrigger(string name)
    {
        foreach (var trigger in this.triggers)
            if (trigger.name.Equals(name))
                trigger.enabled = false;
    }

    public void EnableTrigger(string name)
    {
        foreach (var trigger in this.triggers)
            if (trigger.name.Equals(name))
                trigger.enabled = true;
    }

    public void DisableAxis(string axis)
    {
        foreach (var trigger in this.triggers) trigger.SetControlState(axis, false);
    }

    public void EnableAxis(string axis)
    {
        foreach (var trigger in this.triggers) trigger.SetControlState(axis, true);
    }
}