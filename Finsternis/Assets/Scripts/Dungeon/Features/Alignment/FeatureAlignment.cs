namespace Finsternis
{
using UnityEngine;

    public abstract class FeatureAlignment : ScriptableObject
    {
        public abstract void Align(
            Dungeon dungeon,
            Vector3 dungeonScale,
            Vector2 position, 
            GameObject gObject);
    }
}