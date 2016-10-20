namespace Finsternis
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityQuery;

    public class CardController : MonoBehaviour
    {
        private Image cardImage;
        private Text cardNameField;
        private Text cardCostField;
        private Text cardStackSize;
        private Text newCardLabel;
        private Text attributesNamesField;
        private Text attributesValuesField;

        private CardStack stack;

        public Card Card { get { return this.stack.card; } }

        void Awake() //don't initialize player on awake because it may have not been spawned yet
        {
            this.cardImage = transform.FindDescendant("CardImage").GetComponent<Image>();
            this.cardNameField = transform.FindDescendant("CardNameField").GetComponent<Text>();
            this.cardCostField = transform.FindDescendant("CardCostField").GetComponent<Text>();
            this.cardStackSize = transform.FindDescendant("CardQtdField").GetComponent<Text>();
            this.newCardLabel = transform.FindDescendant("NewCardLabel").GetComponent<Text>();
            this.attributesNamesField = transform.FindDescendant("AttributesNamesField").GetComponent<Text>();
            this.attributesValuesField = transform.FindDescendant("AttributesValuesField").GetComponent<Text>();
        }

        void OnEnable()
        {
            if(this.stack)
                this.newCardLabel.enabled = GameManager.Instance.Player.GetComponent<Inventory>().IsCardNew(this.stack.card);

        }

        public void LoadStack(CardStack c)
        {
            if (c.Equals(this.stack))
                return;
            else if (this.stack)
            {
                this.stack.onCardAdded.RemoveListener(UpdateStackSize);
                this.stack.onCardRemoved.RemoveListener(UpdateStackSize);
            }

            this.stack = c;

            UpdateStackSize();

            this.stack.onCardAdded.AddListener(UpdateStackSize);
            this.stack.onCardRemoved.AddListener(UpdateStackSize);

            this.attributesNamesField.text = "";
            this.attributesValuesField.text = "";

            this.cardImage.sprite = null;
            var sprites = Resources.LoadAll<Sprite>("SPRITE_equipment_icons");
            foreach(var sprite in sprites)
            {
                if (sprite.name.Contains(this.stack.card.MainName.name.ToLower()))
                {
                    this.cardImage.sprite = sprite;
                    break;
                }
            }

            this.newCardLabel.enabled = GameManager.Instance.Player.GetComponent<Inventory>().IsCardNew(this.stack.card);
            this.cardNameField.text = c.card.name;
            this.cardCostField.text = c.card.Cost.ToString();
            foreach (var effect in c.card.GetEffects())
            {
                var modifier = effect as AttributeModifier;
                if (modifier)
                {
                    Append(this.attributesNamesField, modifier.AttributeAlias);
                    Append(this.attributesValuesField, GetValueWithComparison(modifier));
                }
            }
        }

        private void UpdateStackSize()
        {
            this.cardStackSize.enabled = this.stack.Count > 1;
            if (this.cardStackSize.isActiveAndEnabled)
                this.cardStackSize.text = "x" + this.stack.Count;
        }

        private string GetValueWithComparison(AttributeModifier modifier)
        {
            string result = modifier.StringfyValue();
            if (!GameManager.Instance.Player.GetComponent<Inventory>().IsEquipped(this.stack.card))
            {
                var attr = GameManager.Instance.Player.Character.GetAttribute(modifier.AttributeAlias);
                if (attr)
                {
                    float modifiedValue = CalculateModifiedValue(modifier, attr);
                    if (modifiedValue != attr.Value)
                    {
                        result += " (";
                        result += modifiedValue.ToString("n2").Colorize(modifiedValue > attr.Value ? Color.green : Color.red);
                        result += ")";
                    }
                }
            }

            return result;
        }

        private float CalculateModifiedValue(AttributeModifier modifier, EntityAttribute attribute)
        {
            float result = attribute.Value;

            switch (modifier.ChangeType)
            {
                case AttributeModifier.ModifierType.SUM:
                case AttributeModifier.ModifierType.SUBTRACT:
                    result += modifier.ValueChange;
                    break;
                case AttributeModifier.ModifierType.MULTIPLY:
                case AttributeModifier.ModifierType.DIVIDE:
                    result += attribute.BaseValue * modifier.ValueChange;
                    break;
            }

            return Mathf.Max(result, 0); //no need to display negative values
        }

        private void Append(Text field, string text)
        {
            if (field.text.Length == 0)
                field.text = text;
            else
            {
                field.text += "\n" + text;
            }
        }
    }
}
