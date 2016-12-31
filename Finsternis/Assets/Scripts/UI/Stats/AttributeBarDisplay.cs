namespace Finsternis
{
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;

    public class AttributeBarDisplay : MonoBehaviour
    {

        [SerializeField]
        private AttributeTemplate attributeTemplate;

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

        private void GrabAttribute(Attribute attribute)
        {
            if (this.attributeTemplate.Alias.Equals(attribute.Alias))
            {
                attribute.valueChangedEvent += (UpdateDisplay);
            }
        }

        private void UpdateDisplay(Attribute attribute)
        {
            float value = attribute.Value;
            var maxValueConstraint = attribute.Constraints.FirstOrDefault(c => c.Type == AttributeConstraint.AttributeConstraintType.MAX);
            if (maxValueConstraint)
                value /= maxValueConstraint.Value;
            foreach (var image in this.images)
                image.fillAmount = value;
        }
    }
}