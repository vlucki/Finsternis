using UnityEngine;

[CreateAssetMenu(fileName = "InputControl", menuName = "Finsternis/Input/Input Control", order = 1)]
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
    private bool enabled = true;

    [SerializeField]
    private ThresholdTypeEnum thresholdType = ThresholdTypeEnum.DIFFERENT_THAN;

    [Range(-1, 1)]
    [SerializeField]
    private float thresholdValue = 0;

    [Range(0, 10)]
    [SerializeField]
    private float repeatDelay = 0f;

    [SerializeField][Tooltip("Should the value returned by Input.GetAxis be restricted to either 0 or 1?")]
    private bool constantValue = false;

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

    public float RepeatDelay
    {
        get { return this.repeatDelay; }
        set { this.repeatDelay = Mathf.Max(0, value); }
    }

    public virtual float Value()
    {
        return constantValue ? Mathf.Max(Input.GetAxis(Axis)) : Input.GetAxis(Axis);
    }

    public float TrueValue()
    {
        return Input.GetAxis(Axis);
    }

#if UNITY_EDITOR

    void OnValidate()
    {
        this.ThresholdValue = Mathf.Clamp(this.thresholdValue, -1, 1);
    }
#endif
}