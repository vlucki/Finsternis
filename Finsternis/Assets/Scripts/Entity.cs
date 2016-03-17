using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour
{

    private Rigidbody2D body;

    void Start()
    {
        if (!body)
        {
            body = GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {

    }
}
