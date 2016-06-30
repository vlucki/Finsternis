using UnityEngine;
using System.Collections;

public class LerpingTest : MonoBehaviour {

    public bool slerp = false;

    public Vector3 offset = 3*Vector3.down + Vector3.forward;

    [Range(0,1)]
    public float lerpAmount = 0.01f;

    [Range(0, 1)]
    public float threshold = 0.1f;

    private Vector3 _startPos;
    private Vector3 _destination;

    private int direction = 1;

    void Start()
    {
        _startPos = transform.position;
        _destination = _startPos + offset;
    }

    void Update() {
        if (Vector3.Distance(transform.position, _destination) > threshold)
        {
            if (slerp)
                transform.position = Vector3.Slerp(transform.position, _destination, lerpAmount);
            else
                transform.position = Vector3.Lerp(transform.position, _destination, lerpAmount);
        }
        else
        {
            transform.position = _destination;
            direction *= -1;
            _destination += offset * direction;
        }
	}
}
