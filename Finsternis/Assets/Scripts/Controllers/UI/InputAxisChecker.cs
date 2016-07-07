using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class InputAxisChecker : MonoBehaviour
{

    [System.Serializable]
    public class AxisInputEvent : UnityEvent<float>
    {
        public static implicit operator bool(AxisInputEvent evt)
        {
            return evt != null;
        }
    }

    public string[] axisToCheck;


    public AxisInputEvent[] methodsToCall;

    // Update is called once per frame
    void Update()
    {
        if (axisToCheck != null && methodsToCall != null && methodsToCall.Length == axisToCheck.Length)
        {
            for (int i = 0; i < axisToCheck.Length; i++)
            {
                float value = Input.GetAxis(axisToCheck[i]);
                if (value != 0)
                {
                    if (methodsToCall[i])
                        methodsToCall[i].Invoke(value);
                    else
                        Debug.LogWarning("No method assigned to axis " + axisToCheck[i]);
                }
            }
        }
        else
        {
            throw new System.InvalidOperationException("Each axis must correspond to at least one method call!");
        }
    }

}
