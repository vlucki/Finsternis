using UnityEngine;
using System.Collections;

public class FaceCamera : MonoBehaviour {

    private GameObject mainCamera;

    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

	void Update()
    {
        transform.forward = mainCamera.transform.forward;
    }
}
