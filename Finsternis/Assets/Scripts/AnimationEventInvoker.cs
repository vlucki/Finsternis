using UnityEngine;
using UnityEngine.Events;

public class AnimationEventInvoker : MonoBehaviour {

    public UnityEvent onInvoked;
	public void InvokeAnimationEvent()
    {
        onInvoked.Invoke();
    }
}
