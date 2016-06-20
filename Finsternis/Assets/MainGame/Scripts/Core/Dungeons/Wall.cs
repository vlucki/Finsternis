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
    bool shouldFadeCompletely = true;
    public bool canFadeCompletely = false;
    public List<Wall> neighbours;

    float targetAlpha = 1f;
    float fadeAlpha = 0.1f;
    float currentAlpha = 1f;
    int waitFrames = 0;
    private Wall lastNotifier;

    void Awake()
    {
        neighbours = new List<Wall>();
        r = GetComponent<Renderer>();
        m = r.material;
        if (!player)
            player = GameObject.FindGameObjectWithTag("Player");

        if (!mainCamera)
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
    }

    void Start()
    {

        Collider[] colliders = Physics.OverlapBox(transform.position, r.bounds.size + Vector3.one / 2, Quaternion.identity, 1 << LayerMask.NameToLayer("Wall"));
        if (colliders != null && colliders.Length > 0)
        {
            foreach (Collider collider in colliders)
            {
                GameObject neighbour = collider.gameObject;
                if (neighbour != this.gameObject && neighbour != notifier)
                {
                    Wall w = neighbour.GetComponent<Wall>();
                    if (w)
                        neighbours.Add(w);
                }
            }
        }
    }

    public void FadeOut(Wall notifier = null)
    {
        targetAlpha = fadeAlpha;
        waitFrames = 1;

        if (notifier)
            shouldFadeCompletely = canFadeCompletely && notifier.transform.position.z == transform.position.z;
        else if (canFadeCompletely)
            shouldFadeCompletely = true;

        if (shouldFadeCompletely)
        {
            foreach (Wall neighbour in neighbours)
            {
                if (notifier != neighbour)
                    neighbour.FadeOut(this);
            }
        }
        else if (notifier && (!lastNotifier || notifier.transform.position.x == transform.position.x))
        {
            m.SetVector("_FadePoint", notifier.transform.position + (transform.position - notifier.transform.position) / 2);
            Vector3 axis = (notifier.transform.position - transform.position);
            axis.x = Mathf.Abs(axis.x);
            axis.y = Mathf.Abs(axis.y);
            axis.z = Mathf.Abs(axis.z);
            m.SetVector("_FadeAxis", axis);
            lastNotifier = notifier;
        }
    }

    void Update()
    {

        if (!Mathf.Approximately(targetAlpha, currentAlpha))
        {
            currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, 0.05f);
            if (shouldFadeCompletely)
            {
                Color c = m.color;
                c.a = currentAlpha;
                m.color = c;
            }
            else
            {
                m.SetFloat("_FadeAlpha", currentAlpha);
            }
        } else
        {
            currentAlpha = targetAlpha;
        }
    }

    void FixedUpdate()
    {
        if (waitFrames < 0)
        {
            notifier = null;
            lastNotifier = null;
            targetAlpha = 1;
        }
        else
        {
            waitFrames--;
        }
    }
}
