namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.UI;
    using System;
    using UnityQuery;

    public class CharacterSelectionMenu : MenuController
    {
        [SerializeField]
        private Color[] selectedSpritesColors;

        private Button[] characterButtons;

        public void SetPlayer(GameObject prefab)
        {
            GameManager.Instance.SpawnPlayerAtEntrance(prefab);
            BeginClosing();
        }

        protected override void Awake()
        {
            this.characterButtons = GetComponentsInChildren<Button>();
        }

        public void Select(UnityEngine.EventSystems.BaseEventData data)
        {
            this.StopAllCoroutines();
            for (int i = 0; i < this.characterButtons.Length; i++)
            {
                var btn = this.characterButtons[i];
                var characterSprite = btn.transform.GetChild(1).GetChild(1).GetComponent<Image>();

                if (btn.gameObject != data.selectedObject)
                {
                    this.StartCoroutine(_ScaleButton(btn.gameObject, .75f, .5f));
                    characterSprite.color = Color.white / 5;
                }
                else
                {
                    this.StartCoroutine(_ScaleButton(btn.gameObject, 1, 1));
                    characterSprite.color = (!this.selectedSpritesColors.IsNullOrEmpty() && i < this.selectedSpritesColors.Length) ? this.selectedSpritesColors[i] : Color.white;
                }
            }
        }

        private IEnumerator _ScaleButton(GameObject button, float targetScale, float targetAlpha)
        {
            Vector3 finalScale = new Vector3(targetScale, targetScale, targetScale);
            var transform = button.transform;
            var canvas = button.GetComponent<CanvasGroup>();

            float alphaDiff = Mathf.Abs(canvas.alpha - targetAlpha);
            float threshold = alphaDiff * .02f;

            float alphaInterpAmount = .1f;
            float scaleInterpAmount = .1f;

            while (alphaDiff > threshold)
            {
                button.transform.localScale = Vector3.Lerp(button.transform.localScale, finalScale, scaleInterpAmount);
                canvas.alpha = Mathf.Lerp(canvas.alpha, targetAlpha, alphaInterpAmount);
                if (canvas.alpha > targetAlpha)
                    alphaDiff = canvas.alpha - targetAlpha;
                else
                    alphaDiff = targetAlpha - canvas.alpha;

                yield return null;
            }

            button.transform.localScale = finalScale;
            canvas.alpha = targetAlpha;
        }
    }
}