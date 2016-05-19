using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Renderer))]
public class Wall : MonoBehaviour
{
    [Range(1, 200)]
    public float distanceThreshold = 4;
    [Range(1, 100)]
    public float fadeModifier = 20;

    Material m;
    Renderer r;
    GameObject player;
    CameraController mainCamera;
    Wall notifier;
    public List<Wall> neighbours;

    float targetAlpha = 1f;
    float fadeAlpha = 0.1f;
    int waitFrames = 0;

    void Awake()
    {
        neighbours = new List<Wall>();
        r = GetComponent<Renderer>();
        m = r.material;
        if(!player)
            player = GameObject.FindGameObjectWithTag("Player");

        if(!mainCamera)
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
    }

    void Start()
    {
        
        Collider[] colliders = Physics.OverlapBox(transform.position, r.bounds.size + Vector3.one/2, Quaternion.identity, 1 << LayerMask.NameToLayer("Wall"));
        if (colliders != null && colliders.Length > 0)
        {
            foreach (Collider collider in colliders)
            {
                GameObject neighbour = collider.gameObject;
                if (neighbour != this.gameObject && neighbour != notifier)
                {
                    Wall w = neighbour.GetComponent<Wall>();
                    if (w && w.transform.position.z == transform.position.z)
                        neighbours.Add(w);
                }
            }
        }
    }

    public void FadeOut(Wall notifier = null)
    {
        targetAlpha = fadeAlpha;
        waitFrames = 1;
        foreach (Wall neighbour in neighbours)
            if (notifier != neighbour)
                neighbour.FadeOut(this);
    }

    void Update()
    {
        Color c = m.color;

        if(!Mathf.Approximately(targetAlpha, c.a))
        {
            c.a = Mathf.Lerp(c.a, targetAlpha, 0.05f);
            m.color = c;
        }
    }

    void FixedUpdate()
    {
        if (waitFrames < 0)
        {
            if (notifier)
            {
                notifier = null;
            }
            targetAlpha = 1;
        }
        else
        {
            waitFrames--;
        }
    }
}
