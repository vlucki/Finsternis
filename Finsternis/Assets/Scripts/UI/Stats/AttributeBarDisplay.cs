namespace Finsternis
{
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;

    public class AttributeBarDisplay : MonoBehaviour
    {

        [SerializeField]
        private EntityAttribute attributeTemplate;

        [SerializeField]
        private Image[] images;

        void Awake()
        {
            if (!GameManager.Instance.Player)
                GameManager.Instance.onPlayerSpawned += (GrabPlayer);
            else
                GrabPlayer(GameManager.Instance.Player);
        }

        private void GrabPlayer(CharController player)
        {
            GameManager.Instance.onPlayerSpawned -= (GrabPlayer);
            player.Character.onAttributeInitialized.AddListener(GrabAttribute);
        }

        private void GrabAttribute(EntityAttribute attribute)
        {
            if (this.attributeTemplate.Alias.Equals(attribute.Alias))
            {
                attribute.onValueChanged += (UpdateDisplay);
            }
        }

        private void UpdateDisplay(EntityAttribute attribute)
        {
            float value = attribute.Value;
            if (attribute.HasMaximumValue())
                value /= attribute.Max;
            foreach (var image in this.images)
                image.fillAmount = value;
        }
    }
}