namespace Finsternis
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityQuery;

    public class CardDisplay : MonoBehaviour
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
        private bool initialized;
        private bool displayUpdatePending;

        public Card Card { get { return this.stack.card; } }

        void Awake()
        {
            if (!this.initialized)
                Init();
        }

        private void Init()
        {
            if (sprites == null)
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
            this.initialized = true;
        }

        void OnEnable()
        {
            if (this.stack)
            {
                var player = GameManager.Instance.Player;
                if (player)
                {
                    if (!initialized)
                        Init();
                    if (displayUpdatePending)
                    {
                        this.newCardLabel.enabled = player.GetComponent<Inventory>().IsCardNew(this.stack.card);
                        UpdateDisplay();
                    }
                }
#if DEBUG
                else
                {
                    Log.E(this, "Player missing!");
                }
#endif
            }
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

            this.stack.onCardAdded.AddListener(UpdateStackSize);
            this.stack.onCardRemoved.AddListener(UpdateStackSize);

            this.displayUpdatePending = true;

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (!this.initialized)
                return;

            this.displayUpdatePending = false;
            UpdateStackSize();
            UpdateCardImage();
            UpdateCardText();
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

        private void UpdateCardText()
        {
            this.newCardLabel.enabled = GameManager.Instance.Player.GetComponent<Inventory>().IsCardNew(this.stack.card);
            this.cardNameField.text = this.stack.card.name;
            this.cardCostField.text = this.stack.card.Cost.ToString();
            this.attributesNamesField.text = "";
            this.attributesValuesField.text = "";

            var player = GameManager.Instance.Player;
            if (player)
            {
                var effects = this.stack.card.GetEffects();
                var compoundEffects = new Dictionary<EntityAttribute, float>();
                foreach (var effect in effects)
                {
                    var modifier = effect as AttributeModifier;
                    if (modifier)
                    {
                        var attrib = player.Character.GetAttribute(modifier.AttributeAlias);
                        if (!compoundEffects.ContainsKey(attrib))
                            compoundEffects[attrib] = attrib.Value;

                        compoundEffects[attrib] = modifier.GetModifiedValue(attrib.BaseValue, compoundEffects[attrib]);
                    }
                }

                foreach (var compoundEffect in compoundEffects)
                {
                    Append(this.attributesNamesField, compoundEffect.Key.Alias);
                    Append(this.attributesValuesField, GetValueWithComparison(compoundEffect));
                }
            }

        }

        private string GetValueWithComparison(KeyValuePair<EntityAttribute, float> compoundEffect)
        {
            var difference = compoundEffect.Value - compoundEffect.Key.Value;
            string result = difference.ToString("00.00");
            if (difference > 0)
            {
                result = "+" + result + " (" + compoundEffect.Value.ToString("00.00").Colorize(Color.green) + ")";
            }
            else
            {
                result += " (" + compoundEffect.Value.ToString("00.00").Colorize(Color.red) + ")";
            }

            return result;
        }

        private string GetValueWithComparison(AttributeModifier modifier)
        {
            string result = modifier.StringfyValue();
            var player = GameManager.Instance.Player;
            if (!player)
            {
#if DEBUG
                Log.E(this, "Player missing!");
#endif
                return null;
            }

            if (!player.GetCachedComponent<Inventory>().IsEquipped(this.stack.card))
            {
                var attr = player.Character.GetAttribute(modifier.AttributeAlias);
                if (attr)
                {
                    float modifiedValue = modifier.GetModifiedValue(attr.BaseValue, attr.Value);
                    string valueStr = modifiedValue.ToString("00.00");
                    if (modifiedValue != attr.Value)
                    {
                        result += " (";
                        result += valueStr.Colorize(modifiedValue > attr.Value ? Color.green : Color.red);
                        result += ")";
                    }
                }
            }

            return result;
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
