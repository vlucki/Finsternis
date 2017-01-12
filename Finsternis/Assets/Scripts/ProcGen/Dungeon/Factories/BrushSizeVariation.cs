namespace Finsternis
{
    using System;
    using UnityEngine;

    [Serializable]
    public class BrushSizeVariation
    {
        [SerializeField]
        [Range(1, 100)]
        private int minWidth;
        [SerializeField]
        [Range(1, 100)]
        private int maxWidth;
        [SerializeField]
        [Range(1, 100)]
        private int minHeight;
        [SerializeField]
        [Range(1, 100)]
        private int maxHeight;

        public Vector2 Min
        {
            get { return new Vector2(minWidth, minHeight); }
        }

        public Vector2 Max
        {
            get { return new Vector2(maxWidth, maxHeight); }
        }

        public BrushSizeVariation(int minWidth, int maxWidth, int minHeight, int maxHeight)
        {
            this.minWidth = minWidth;
            this.maxWidth = maxWidth;
            this.minHeight = minHeight;
            this.maxHeight = maxHeight;
        }

        public void Validate(int maxWidth, int maxHeight)
        {
            this.maxWidth = Mathf.Clamp(this.maxWidth, 0, maxWidth);
            this.minWidth = Mathf.Clamp(this.minWidth, 0, this.maxWidth);

            this.maxHeight = Mathf.Clamp(this.maxHeight, 0, maxHeight);
            this.minHeight = Mathf.Clamp(this.minHeight, 0, this.maxHeight);
        }
    }
}