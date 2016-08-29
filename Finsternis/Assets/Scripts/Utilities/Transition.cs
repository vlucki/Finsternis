namespace Finsternis
{
  using UnityEngine;
  using UnityEngine.Events;
  
  public abstract class Transition : MonoBehaviour
  {
      public bool skippable = false;
  
      public bool beginOnAwake = false;
  
      public UnityEvent OnTransitionStarted;
      public UnityEvent OnTransitionEnded;
  
      private bool transitioning;
  
      public bool Transitioning { get { return this.transitioning; } }
  
      protected virtual void Awake()
      {
          if (beginOnAwake)
              Begin();
      }
  
      public void Begin()
      {
          if (!transitioning)
          {
              this.transitioning = true;
              OnTransitionStarted.Invoke();
          }
      }
  
      protected void End()
      {
          if (transitioning)
          {
              this.transitioning = false;
              OnTransitionEnded.Invoke();
          }
      }
  
      public void Skip()
      {
          if (skippable)
          {
              End();
          }
      }
  
  }
}
