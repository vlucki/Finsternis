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
                    this.StartCoroutine(_ScaleButton(btn.gameObject, .75f, .5f));
                    btn.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.white/5;
                }
                else
                {
                    this.StartCoroutine(_ScaleButton(btn.gameObject, 1, 1));
                    btn.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.white;
                }
            }
        }

        private IEnumerator _ScaleButton(GameObject button, float targetScale, float targetAlpha)
        {
            Vector3 finalScale = new Vector3(targetScale, targetScale, targetScale);
            var canvas = button.GetComponent<CanvasGroup>();
            while (button.transform.localScale != finalScale)
            {
                button.transform.localScale = Vector3.Lerp(button.transform.localScale, finalScale, .2f);
                canvas.alpha = Mathf.Lerp(canvas.alpha, targetAlpha, .2f);
                yield return null;
            }
        }
    }
}