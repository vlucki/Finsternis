namespace Finsternis
{
  using UnityEngine;
  using System.Collections;
  using UnityEngine.UI;
  
  [RequireComponent(typeof(Graphic))]
  public class FadeTransition : Transition
  {
      [SerializeField][Range(0, 10)]
      private float fadeTime = 2;
  
      [SerializeField][Range(0,1)]
      private int targetAlpha = 1;
      
      private Graphic graphicToFade;
  
      protected override void Awake()
      {
          graphicToFade = GetComponent<Graphic>();
  
          if (OnTransitionStarted == null)
              OnTransitionStarted = new UnityEngine.Events.UnityEvent();
  
          OnTransitionStarted.AddListener(() => StartCoroutine(_DoFade()));
  
          graphicToFade.canvasRenderer.SetAlpha(1 - targetAlpha);
  
          base.Awake();
      }
  
      private IEnumerator _DoFade()
      {
          graphicToFade.CrossFadeAlpha(targetAlpha, fadeTime, false);
          yield return new WaitForSeconds(fadeTime);
  
          End();
      }
  }
}
