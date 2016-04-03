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

    public void Start()
    {
        ValidateState();
        txtName.text = character.name;
    }

    void Update()
    {
        UpdateHud();
    }

    private void UpdateHud()
    {
        
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

