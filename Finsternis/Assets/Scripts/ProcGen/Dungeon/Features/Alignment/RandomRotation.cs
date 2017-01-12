namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;
    using Random = UnityEngine.Random;

    [CreateAssetMenu(fileName = "RandomRotation", menuName = "Finsternis/Features/Alignment/Rotate")]
    public class RandomRotation : FeatureAlignment
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
        private Range3F variation = new Range3F(Vector3.zero, new Vector3(0, 360, 0));

        [SerializeField, Range(1, 180)]
        private int step = 45;

        public override void Align(Dungeon dungeon, Vector3 dungeonScale, Vector2 position, GameObject gObject, int count = 0)
        {
            if (this.variation.min == this.variation.max)
                return;
            Vector3 angles = this.variation.min;
            if (this.variation.min.x != this.variation.max.x)
                angles.x = Random.Range(variation.min.x, variation.max.x);

            if (this.variation.min.y != this.variation.max.y)
                angles.y = Random.Range(variation.min.y, variation.max.y);

            if (this.variation.min.z != this.variation.max.z)
                angles.z = Random.Range(variation.min.z, variation.max.z);


            if (step > 1)
            {
                angles.x = Mathf.Round(angles.x / step) * step;
                angles.y = Mathf.Round(angles.y / step) * step;
                angles.z = Mathf.Round(angles.z / step) * step;
            }

            gObject.transform.Rotate(angles);
        }
    }
}