namespace Finsternis
{
    using UnityEngine;

    public class StatsPanelController : MenuController
    {
        [SerializeField]
        private GameObject displayPrefab;
        private bool initialized = false;

        private void Start()
        {
            if (GameManager.Instance.Player)
                Init(GameManager.Instance.Player);
            else
                GameManager.Instance.onPlayerSpawned.AddListener(Init);
        }

        private void Init(CharController player)
        {
            GameManager.Instance.onPlayerSpawned.RemoveListener(Init);
            if (initialized)
                return;
            initialized = true;
            foreach (var attribute in player.Character)
            {
                GameObject display = (GameObject)Instantiate(displayPrefab, transform);
                display.transform.localScale = Vector3.one;
                display.GetComponent<AttributeDisplay>().SetAttribute(attribute);
            }
        }
    }
}