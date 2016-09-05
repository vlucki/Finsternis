namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;
    using UnityEngine.UI;

    public class StatsPanelController : MenuController
    {

        Entity player;
        protected override void Awake()
        {
            base.Awake();
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Entity>();

            foreach (var attribute in player.Attributes)
            {
                attribute.onValueChanged.AddListener(UpdateAttributeDisplay);
                UpdateAttributeDisplay(attribute);
            }
        }

        private void UpdateAttributeDisplay(EntityAttribute attribute)
        {
            var child = transform.FindChild(attribute.Alias);
            if (child)
                child.GetComponentInChildren<Text>().text = attribute.ToString();
        }
    }
}