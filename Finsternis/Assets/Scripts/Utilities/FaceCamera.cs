using UnityEngine;
using System.Collections;

public class FaceCamera : MonoBehaviour {

    private GameObject camera;

    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

	void Update()
    {
        transform.forward = camera.transform.forward;
    }
}
