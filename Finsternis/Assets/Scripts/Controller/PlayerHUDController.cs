using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDController : MonoBehaviour
{
    [SerializeField]
    private Entity _character;

    [SerializeField]
    private Text _txtName;

    [SerializeField]
    private Text _txtLvl;

    [SerializeField]
    private Text _txtHP;

    [SerializeField]
    private Text _txtMP;

    private RangedValueAttribute _health;
    private RangedValueAttribute _mana;

    void Awake()
    {
        if(!_txtName || !_txtLvl || !_txtHP || !_txtMP)
        {
            Text[] fields = GetComponentsInChildren<Text>();
            foreach(Text field in fields)
            {
                if (field.name.Equals("txtName"))
                    _txtName = field;
                else if (field.name.Equals("txtLvl"))
                    _txtLvl = field;
                else if (field.name.Equals("txtHP"))
                    _txtHP = field;
                else if(field.name.Equals("txtMP"))
                    _txtMP = field;
            }
        }

        ValidateState();
    }

    public void Start()
    {
        UpdateHud();
    }

    void Update()
    {
        UpdateHud();
    }

    private void UpdateHud()
    {
        if (_character)
        {
            if (String.IsNullOrEmpty(_txtName.text))
                _txtName.text = _character.name;

            _health = UpdateRangedField("hp", _health, _txtHP);
            _mana = UpdateRangedField("mp", _mana, _txtMP);
        }
    }

    private RangedValueAttribute UpdateRangedField(string name, RangedValueAttribute attribute, Text textField)
    {
        if (!attribute)
        {
            attribute = _character.GetAttribute(name) as RangedValueAttribute;
        }
        if (attribute)
        {
            textField.text = attribute.Value + "/" + attribute.Max;
        }
        return attribute;
    }

    private void ValidateState()
    {
        if (!_character)
            throw new System.InvalidOperationException("Must assign a Character to the HUD.");

        if (!_txtName)
            throw new System.InvalidOperationException("Must assign a Text for the character name.");

        if (!_txtLvl)
            throw new System.InvalidOperationException("Must assign a Text for the character level");

        if (!_txtHP)
            throw new System.InvalidOperationException("Must assign a Text for the character HP.");

        if (!_txtMP)
            throw new System.InvalidOperationException("Must assign a Text for the character MP.");
    }
}

