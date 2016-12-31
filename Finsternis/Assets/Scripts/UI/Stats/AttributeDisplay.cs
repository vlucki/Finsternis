namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using UnityEngine.UI;
    using Extensions;

    public class AttributeDisplay : MonoBehaviour
    {
        [System.Serializable]
        public class AttributeMapping
        {
            public string attributeAlias;
            public Sprite attributeGraphic;

            public static implicit operator bool(AttributeMapping am)
            {
                return am != null;
            }
        }

        [SerializeField]

        private List<AttributeMapping> mappedAttributes;
        [SerializeField]
        private AttributeTemplate observedAttributeTemplate;

        [SerializeField]
        private Text attributeAliasLabel;

        [SerializeField]
        private Text attributeValueLabel;

        [SerializeField]
        private Image attributeImage;

        private Attribute observedAttribute;

        public void SetAttribute(Attribute attribute)
        {
            if (this.observedAttribute)
                this.observedAttribute.valueChangedEvent -= AtttributeValueChanged;

            this.observedAttribute = attribute;
            this.observedAttribute.valueChangedEvent += AtttributeValueChanged;

            this.attributeAliasLabel.text = this.observedAttribute.Alias;
            this.attributeValueLabel.text = this.observedAttribute.Value.ToString("n2");

            var mappedAttribute = mappedAttributes.Find((map) => map.attributeAlias.Equals(this.observedAttribute.Alias));
            if (mappedAttribute)
            {
                this.attributeAliasLabel.alignment = TextAnchor.MiddleRight;
                this.attributeImage.transform.parent.gameObject.SetActive(true);
                this.attributeImage.overrideSprite = mappedAttribute.attributeGraphic;
            }
            else
            {
                this.attributeAliasLabel.alignment = TextAnchor.MiddleCenter;
                this.attributeImage.transform.parent.gameObject.SetActive(false);
            }

        }

        private void AtttributeValueChanged(Attribute attribute)
        {
            this.attributeValueLabel.text = attribute.Value.ToString("n2");
        }
    }
}