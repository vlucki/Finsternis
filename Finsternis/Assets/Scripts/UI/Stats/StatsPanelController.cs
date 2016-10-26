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

        void Start()
        {
            if (GameManager.Instance.Player)
                Init();
            else
                GameManager.Instance.OnPlayerSpawned.AddListener(Init);
        }

        private void Init()
        {
            GameManager.Instance.OnPlayerSpawned.RemoveListener(Init);
            if (initialized)
                return;
            initialized = true;
            foreach (var attribute in GameManager.Instance.Player.Character)
            {
                GameObject display = (GameObject)Instantiate(displayPrefab, transform);
                display.transform.localScale = Vector3.one;
                display.GetComponent<AttributeDisplay>().SetAttribute(attribute);
            }
        }
    }
}