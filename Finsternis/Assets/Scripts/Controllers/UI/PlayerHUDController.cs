namespace Finsternis
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class PlayerHUDController : MonoBehaviour
    {
        [SerializeField]
        private Entity character;

        [SerializeField]
        private Text txtName;

        [SerializeField]
        private Text txtLvl;

        [SerializeField]
        private Text txtHP;

        [SerializeField]
        private Text txtMP;

        private EntityAttribute health;
        private EntityAttribute mana;

        void Awake()
        {
            if (!this.txtName || !this.txtLvl || !this.txtHP || !this.txtMP)
            {
                Text[] fields = GetComponentsInChildren<Text>();
                foreach (Text field in fields)
                {
                    if (field.name.Equals("txtName"))
                        this.txtName = field;
                    else if (field.name.Equals("txtLvl"))
                        this.txtLvl = field;
                    else if (field.name.Equals("txtHP"))
                        this.txtHP = field;
                    else if (field.name.Equals("txtMP"))
                        this.txtMP = field;
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
            if (this.character)
            {
                if (String.IsNullOrEmpty(this.txtName.text))
                    this.txtName.text = this.character.name;

                this.health = UpdateRangedField("hp", this.health, this.txtHP);
                this.mana = UpdateRangedField("mp", this.mana, this.txtMP);
            }
        }

        private EntityAttribute UpdateRangedField(string name, EntityAttribute attribute, Text textField)
        {
            if (!attribute)
            {
                attribute = this.character.GetAttribute(name) as EntityAttribute;
            }
            if (attribute)
            {
                textField.text = attribute.Value + "/" + attribute.Max;
            }
            return attribute;
        }

        private void ValidateState()
        {
            if (!this.character)
                throw new System.InvalidOperationException("Must assign a Character to the HUD.");

            if (!this.txtName)
                throw new System.InvalidOperationException("Must assign a Text for the character name.");

            if (!this.txtLvl)
                throw new System.InvalidOperationException("Must assign a Text for the character level");

            if (!this.txtHP)
                throw new System.InvalidOperationException("Must assign a Text for the character HP.");

            if (!this.txtMP)
                throw new System.InvalidOperationException("Must assign a Text for the character MP.");
        }
    }

}