using ByteSheep.Events;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityQuery;

public class InputAxisChecker : MonoBehaviour
{
    [System.Serializable]
    public class AxisInputEvent : QuickEvent<float>
    {
        public static implicit operator bool(AxisInputEvent evt)
        {
            return evt != null;
        }
    }

    [System.Serializable]
    public class AxesToCheck
    {
        [AxesName]
        public string _axis;

        public string Axis { get { return _axis; } }

        public AxisInputEvent onAxisActive;
    }

    [SerializeField]
    private AxesToCheck[] _axesToCheck;

    // Update is called once per frame
    void Update()
    {
        if (_axesToCheck != null && _axesToCheck.Length > 0)
        {
            for (int i = 0; i < _axesToCheck.Length; i++)
            {
                float value = Input.GetAxis(_axesToCheck[i].Axis);
                if (value != 0)
                {
                    if (_axesToCheck[i].onAxisActive)
                        _axesToCheck[i].onAxisActive.Invoke(value);
                    else
                        Log.Warn("No method assigned to axis " + _axesToCheck[i].Axis);

                }
            }
        }
        else
        {
            throw new System.InvalidOperationException("Each axis must correspond to at least one method call!");
        }
    }

}