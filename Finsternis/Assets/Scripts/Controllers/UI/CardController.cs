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

        private Entity player;

        private Card card;

        public Card Card { get { return this.card; } }

        void Awake()
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
                var playerGO = GameObject.FindGameObjectWithTag("Player");
                if (!playerGO)
                    return null;
                this.player = playerGO.GetComponent<Entity>();
            }
            return this.player;
        }

        void LoadCard(Card c)
        {
            this.card = c;
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
            if (GetPlayer())
            {
                var attr = this.player.GetAttribute(modifier.AttributeAlias);
                if (attr)
                {
                    float modifiedValue = CalculateModifiedValue(modifier, attr.Value);
                    if(modifiedValue != attr.Value)
                    {
                        result += "(";
                        result += Colorize(modifiedValue.ToString(), modifiedValue > attr.Value ? Color.green : Color.red);
                        result += ")";
                    }
                }
            }

            return result;
        }

        private string Colorize(string s, Color c)
        {
            return "<color=#" + Mathf.RoundToInt(c.r * 255).ToString("X") 
                              + Mathf.RoundToInt(c.g * 255).ToString("X") 
                              + Mathf.RoundToInt(c.b * 255).ToString("X") 
                              + Mathf.RoundToInt(c.a * 255).ToString("X") + ">" + s + "</color>";
        }

        private float CalculateModifiedValue(AttributeModifier modifier, float value)
        {
            float result = value;

            switch (modifier.ChangeType)
            {
                case AttributeModifier.ModifierType.Absolute:
                    result += modifier.ValueChange;
                    break;
                case AttributeModifier.ModifierType.Relative:
                    result *= modifier.ValueChange;
                    break;
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