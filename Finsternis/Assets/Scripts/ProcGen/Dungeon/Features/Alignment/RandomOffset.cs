namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;
    using Random = UnityEngine.Random;

    [CreateAssetMenu(fileName = "RandomOffset", menuName = "Finsternis/Features/Alignment/Offset")]
    public class RandomOffset : FeatureAlignment
    {
        [Serializable]
        public struct Range3F
        {
            public Vector3 min;
            public Vector3 max;

            public Range3F(Vector3 min, Vector3 max)
            {
                this.min = min;
                this.max = max;
            }
        }

        [SerializeField]
        private Range3F variation = new Range3F(new Vector3(-1, 0, -1), new Vector3(1, 0, 1));

        public override void Align(Dungeon dungeon, Vector3 dungeonScale, Vector2 position, GameObject gObject, int count = 0)
        {
            if (this.variation.min == this.variation.max)
                return;
            Vector3 offset = this.variation.min;
            if (this.variation.min.x != this.variation.max.x)
                offset.x = Random.Range(variation.min.x, variation.max.x);
            if (this.variation.min.y != this.variation.max.y)
                offset.y = Random.Range(variation.min.y, variation.max.y);
            if (this.variation.min.z != this.variation.max.z)
                offset.z = Random.Range(variation.min.z, variation.max.z);

            gObject.transform.Translate(offset);
        }
    }
}