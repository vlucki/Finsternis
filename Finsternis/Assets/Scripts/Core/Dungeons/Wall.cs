using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class Wall : MonoBehaviour
{
    [Range(1, 200)]
    public float distanceThreshold = 20;
    Material m;
    Renderer r;
    GameObject player;
    CameraController mainCamera;

    void Awake()
    {
        r = GetComponent<Renderer>();
        m = r.material;
        if(!player)
            player = GameObject.FindGameObjectWithTag("Player");

        if(!mainCamera)
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
    }

    void Update()
    {

        Color c = m.color;
        float newAlpha = 1;
        if (mainCamera.OccludingObject != null && transform.position.z < player.transform.position.z)
        {
            float dist = Mathf.Abs(transform.position.x - player.transform.position.x);
            if(dist <= 4)
                newAlpha = Mathf.Clamp(dist*dist / distanceThreshold, 0.1f, 1);
        }

        if (!Mathf.Approximately(newAlpha, c.a))
        {
            c.a = Mathf.Lerp(c.a, newAlpha, 0.05f);
            m.color = c;
        }
    }
}
