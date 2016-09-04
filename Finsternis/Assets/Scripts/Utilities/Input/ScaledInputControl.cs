using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "CustomInputControl", menuName = "Finsternis/Input/Scaled Input", order = 1)]
public class ScaledInputControl : InputControl
{
    [SerializeField]
    [Range(0, 100)]
    private float valueMultiplier = 1;

    public override float Value
    {
        get { return base.Value * valueMultiplier; }
    }
}
