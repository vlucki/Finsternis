using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "CustomInputControl", menuName = "Finsternis/Input/Custom Input Control", order = 1)]
public class CustomInputControl : InputControl
{
    [SerializeField]
    [Range(0, 100)]
    private float valueMultiplier = 1;

    public override float Value()
    {
        return base.Value() * valueMultiplier;
    }
}
