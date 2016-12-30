namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.UI;
    
    public class AttributeBarDisplay : MonoBehaviour
    {

        [SerializeField]
        private EntityAttribute attribute;

        [SerializeField]
        private Image[] images;

        void Awake()
        {
            if (!GameManager.Instance.Player)
                GameManager.Instance.onPlayerSpawned.AddListener(GrabPlayer);
            else
                GrabPlayer(GameManager.Instance.Player);
        }

        private void GrabPlayer(CharController player)
        {
            GameManager.Instance.onPlayerSpawned.RemoveListener(GrabPlayer);
            player.Character.onAttributeInitialized.AddListener(GrabAttribute);
        }

        private void GrabAttribute(EntityAttribute attribute)
        {
            if (attribute.Alias.Equals(this.attribute.Alias))
            {
                attribute.onValueChanged.AddListener(UpdateDisplay);
            }
        }

        private void UpdateDisplay(EntityAttribute attribute)
        {
            float percentage = attribute.Value / attribute.Max;
            foreach (var image in this.images)
                image.fillAmount = percentage;
        }
    }
}