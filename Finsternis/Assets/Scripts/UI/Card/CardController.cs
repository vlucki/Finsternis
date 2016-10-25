namespace Finsternis
{
    using System;
    using System.Collections.Generic;
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

        private static Sprite[] sprites;

        public Card Card { get { return this.stack.card; } }
        
        void Awake() //don't initialize player on awake because it may have not been spawned yet
        {
            if(sprites == null)
            {
                sprites = Resources.LoadAll<Sprite>("Sprites/card_sprites");
            }
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


            UpdateCardImage();
            UpdateCardText();
        }

        private void UpdateCardText()
        {
            this.newCardLabel.enabled = GameManager.Instance.Player.GetComponent<Inventory>().IsCardNew(this.stack.card);
            this.cardNameField.text = this.stack.card.name;
            this.cardCostField.text = this.stack.card.Cost.ToString();
            this.attributesNamesField.text = "";
            this.attributesValuesField.text = "";

            foreach (var effect in this.stack.card.GetEffects())
            {
                var modifier = effect as AttributeModifier;
                if (modifier)
                {
                    Append(this.attributesNamesField, modifier.AttributeAlias);
                    Append(this.attributesValuesField, GetValueWithComparison(modifier));
                }
            }
        }

        private void UpdateCardImage()
        {
            this.cardImage.sprite = null;
            
            List<Sprite> usableSprites = new List<Sprite>();
            foreach (var sprite in sprites)
            {
                if (sprite.name.ToLower().Contains(this.stack.card.MainName.name.ToLower()))
                {
                    usableSprites.Add(sprite);
                }
            }
            if (usableSprites.Count > 0)
            {
                this.cardImage.Enable();
                this.cardImage.sprite = usableSprites.GetRandom(UnityEngine.Random.Range);
            }
            else
            {
                this.cardImage.Disable();
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

            switch (modifier.TypeOfModifier)
            {
                case AttributeModifier.ModifierType.SUM:
                    result += modifier.ValueChange;
                    break;
                case AttributeModifier.ModifierType.SUBTRACT:
                    result -= modifier.ValueChange;
                    break;
                case AttributeModifier.ModifierType.DIVIDE:
                    result += attribute.BaseValue / modifier.ValueChange;
                    break;
                case AttributeModifier.ModifierType.MULTIPLY:
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
