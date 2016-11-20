namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.UI;
    using System;

    [RequireComponent(typeof(Image))]
    public class AttributeBarDisplay : MonoBehaviour
    {

        [SerializeField]
        private EntityAttribute attribute;

        private Image image;

        void Awake()
        {
            GameManager.Instance.onPlayerSpawned.AddListener(GrabPlayer);
            this.image = GetComponent<Image>();
            this.image.type = Image.Type.Filled;
            this.image.fillMethod = Image.FillMethod.Horizontal;
            this.gameObject.SetActive(false);
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
                this.gameObject.SetActive(true);
            }
        }

        private void UpdateDisplay(EntityAttribute attribute)
        {
            this.image.fillAmount = attribute.Value / attribute.Max;
        }
    }
}