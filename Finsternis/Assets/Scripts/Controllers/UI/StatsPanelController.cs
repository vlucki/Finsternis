namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;
    using UnityEngine.UI;
    using UnityQuery;

    public class StatsPanelController : MenuController
    {
        [SerializeField]
        private GameObject rowPrefab;
        private bool initialized = false;
        Entity player;
        protected override void Awake()
        {
            base.Awake();
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();
            OnOpen.AddListener(UpdateAttributes);
        }

        void Start()
        {
            if (initialized)
                return;
            initialized = true;
            foreach (var attribute in player.Attributes)
            {
                attribute.onValueChanged.AddListener(UpdateAttributeDisplay);
                UpdateAttributeDisplay(attribute);
            }
        }

        public void UpdateAttributes()
        {
            foreach (var attribute in player.Attributes)
                UpdateAttributeDisplay(attribute);
        }

        private void UpdateAttributeDisplay(EntityAttribute attribute)
        {
            var child = transform.FindDescendant(attribute.Alias);
            if (!child)
            {
                child = AddRow(attribute);
            }
            child.GetComponentInChildren<Text>().text = attribute.Value.ToString();
        }

        private Transform AddRow(EntityAttribute attribute)
        {
            GameObject row = (GameObject)Instantiate(rowPrefab, transform);
            row.name = attribute.Alias + "Row";
            var attrName = row.transform.FindChild("attributeName");
            attrName.GetComponent<Text>().text = attribute.Alias;
            var attrValue = row.transform.FindChild("attributeValue");
            attrValue.name = attribute.Alias;
            return attrValue;
        }
    }
}