namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityQuery;

    public class CardController : MonoBehaviour
    {
        private Text cardNameField;
        private Text cardCostField;
        private Text attributesNamesField;
        private Text attributesValuesField;

        private Character player;

        private Card card;

        public Card Card { get { return this.card; } }

        void Awake() //don't initialize player on awake because it may have not been spawned yet
        {
            this.cardNameField = transform.FindDescendant("CardNameField").GetComponent<Text>();
            this.cardCostField = transform.FindDescendant("CardCostField").GetComponent<Text>();
            this.attributesNamesField = transform.FindDescendant("AttributesNamesField").GetComponent<Text>();
            this.attributesValuesField = transform.FindDescendant("AttributesValuesField").GetComponent<Text>();
        }

        private Entity GetPlayer()
        {
            if (!this.player)
            {
                var playerGO = GameManager.Instance.Player;
                if (!playerGO)
                    return null;
                this.player = playerGO.Character;
            }
            return this.player;
        }

        public void LoadCard(Card c)
        {
            if (c.Equals(this.card))
                return;
            this.card = c;
            this.attributesNamesField.text = "";
            this.attributesValuesField.text = "";
            this.cardNameField.text = c.name;
            this.cardCostField.text = c.Cost.ToString();
            foreach(var effect in c.GetEffects())
            {
                var modifier = effect as AttributeModifier;
                if (modifier)
                {
                    Append(this.attributesNamesField, modifier.AttributeAlias);
                    Append(this.attributesValuesField, GetValueWithComparison(modifier));
                }
            }
        }

        private string GetValueWithComparison(AttributeModifier modifier)
        {
            string result = modifier.StringfyValue();
            if (GetPlayer() && !this.player.GetComponent<Inventory>().IsEquipped(this.card))
            {
                var attr = this.player.GetAttribute(modifier.AttributeAlias);
                if (attr)
                {
                    float modifiedValue = CalculateModifiedValue(modifier, attr);
                    if(modifiedValue != attr.Value)
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
                case AttributeModifier.ModifierType.Absolute:
                    result += modifier.ValueChange;
                    break;
                case AttributeModifier.ModifierType.Relative:
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
