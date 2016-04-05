using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public Character character;
    public Text txtName;
    public Text txtLvl;
    public Text txtHP;
    public Text txtMP;

    private RangedValueAttribute health;
    private RangedValueAttribute mana;
    private AttributeTable table;

    public void Start()
    {
        ValidateState();
        txtName.text = character.name;

        table = character.Attributes;

        UpdateHud();
    }

    void Update()
    {
        UpdateHud();
    }

    private void UpdateHud()
    {
        UpdateRangedField("health", ref health, txtHP);
        UpdateRangedField("mana", ref mana, txtMP);
    }

    private void UpdateRangedField(string name, ref RangedValueAttribute attribute, Text textField)
    {
        if (attribute == null)
        {
            attribute = table.GetAttribute(name) as RangedValueAttribute;
        }
        if (attribute != null)
        {
            textField.text = attribute.Value + "/" + attribute.Max;
        }
    }

    private void ValidateState()
    {
        if (!character)
            throw new System.InvalidOperationException("Must assign a Character to the HUD.");

        if (!txtName)
            throw new System.InvalidOperationException("Must assign a Text for the character name.");

        if (!txtLvl)
            throw new System.InvalidOperationException("Must assign a Text for the character level");

        if (!txtHP)
            throw new System.InvalidOperationException("Must assign a Text for the character HP.");

        if (!txtMP)
            throw new System.InvalidOperationException("Must assign a Text for the character MP.");
    }
}

