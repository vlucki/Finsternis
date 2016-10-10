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
        }

        void Start()
        {
            if (initialized)
                return;
            initialized = true;
            foreach (var attribute in player)
            {
                GameObject display = (GameObject)Instantiate(displayPrefab, transform);
                display.GetComponent<AttributeDisplay>().SetAttribute(attribute);
            }
        }
    }
}