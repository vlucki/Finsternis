using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField]
    [Range(0, 999)]
    private float remainingTime = 1f;

    public UnityEvent onCountdownFinished;

    void Update()
    {
        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0)
        {
            onCountdownFinished.Invoke();
            this.enabled = false;
        }
    }
}
