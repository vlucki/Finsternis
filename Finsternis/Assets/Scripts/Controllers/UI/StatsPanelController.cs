namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityQuery;

    public class StatsPanelController : MenuController
    {
        [SerializeField]
        private GameObject displayPrefab;
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

        public override void BeginClosing()
        {
            base.BeginClosing();
        }

        public void UpdateAttributes()
        {
            foreach (var attribute in player.Attributes)
                UpdateAttributeDisplay(attribute);
        }

        /// <summary>
        /// Updates the display corresponding to an attribute. If not display is found, a new one is created.
        /// </summary>
        /// <param name="attribute">The attribute to be displayed.</param>
        private void UpdateAttributeDisplay(EntityAttribute attribute)
        {
            var child = transform.FindDescendant(attribute.Alias);
            if (!child)
            {
                child = AddDisplay(attribute);
            }
            child.GetComponentInChildren<Text>().text = attribute.Value.ToString();
        }

        /// <summary>
        /// Adds a new item to the panel to display an attribute.
        /// </summary>
        /// <param name="attribute">The attribute to be displayed.</param>
        /// <returns>The transform of the component that will display the attribute value.</returns>
        private Transform AddDisplay(EntityAttribute attribute)
        {
            GameObject display = (GameObject)Instantiate(displayPrefab, transform);
            display.name = attribute.Alias + "Display";

            var attrName = display.transform.FindChild("attributeName");
            attrName.GetComponent<Text>().text = attribute.Alias;

            var attrValueField = display.transform.FindChild("attributeValue");
            attrValueField.name = attribute.Alias;
            return attrValueField;
        }
    }
}