namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.UI;

    public class CharacterSelectionMenu : MenuController
    {
        private Button[] buttons;
        public void SetPlayer(GameObject prefab)
        {
            GameManager.Instance.SpawnPlayerAtEntrance(prefab);
            BeginClosing();
        }

        protected override void Awake()
        {
            this.buttons = GetComponentsInChildren<Button>();
        }

        public void Select(UnityEngine.EventSystems.BaseEventData data)
        {
            this.StopAllCoroutines();
            foreach(var btn in this.buttons)
            {
                if(btn.gameObject != data.selectedObject)
                {
                    this.StartCoroutine(_ScaleButton(btn.gameObject, 0.75f));
                    var color = btn.targetGraphic.color;
                    color.a = .5f;
                    btn.targetGraphic.color = color;
                }
                else
                {
                    this.StartCoroutine(_ScaleButton(btn.gameObject, 1));
                    var color = btn.targetGraphic.color;
                    color.a = 1;
                    btn.targetGraphic.color = color;
                }
            }
        }

        private IEnumerator _ScaleButton(GameObject button, float targetScale)
        {
            Vector3 finalScale = new Vector3(targetScale, targetScale, targetScale);
            while (button.transform.localScale != finalScale)
            {
                button.transform.localScale = Vector3.Lerp(button.transform.localScale, finalScale, 0.2f);
                yield return null;
            }
        }
    }
}