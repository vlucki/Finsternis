using UnityEngine;
using System;

[Serializable]
public struct Circle
{
    public readonly float radius;
    public readonly Vector2 center;

    public Circle(float radius, Vector2 center)
    {
        this.radius = radius;
        this.center = center;
    }

    public bool Contains(Vector2 point)
    {
        return Vector2.Distance(point, center) <= radius;
    }

    public Vector2 ProjectPoint(Vector2 point)
    {
        return Vector2.MoveTowards(this.center, point, this.radius);
    }
}