namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.UI;
    
    public class AttributeBarDisplay : MonoBehaviour
    {

        [SerializeField]
        private EntityAttribute attribute;

        [SerializeField]
        private Image image;

        void Awake()
        {
            GameManager.Instance.onPlayerSpawned.AddListener(GrabPlayer);
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
            this.image.fillAmount = attribute.Value / attribute.Max;
        }
    }
}