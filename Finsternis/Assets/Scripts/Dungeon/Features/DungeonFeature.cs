namespace Finsternis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityQuery;

    [CreateAssetMenu(fileName = "DungeonFeature", menuName = "Finsternis/Dungeon/Features/Generic Feature", order = 0)]
    public class DungeonFeature : ScriptableObject
    {
        public enum FeatureType
        {
            ADD_ON = 0,
            REPLACEMENT = 1 //should this feature replace the tile it's on?
        }

        [Serializable]
        public struct AlignmentParameters
        {
            public bool alignToWall;
            public Vector3 minOffset;
            public Vector3 maxOffset;
            public int faceOffset;

            public AlignmentParameters(Vector3 minOffset, Vector3 maxOffset, int faceOffset, bool alignToWall)
            {
                this.minOffset = minOffset;
                this.maxOffset = maxOffset;
                this.faceOffset = faceOffset;
                this.alignToWall = alignToWall;
            }
        }

        [Serializable]
        public struct PerimeterRestriction
        {
            public bool above;
            public bool below;
            public bool left;
            public bool right;
            public List<DungeonFeature> features;
        }

        #region private variables
        [SerializeField]
        [Range(0.01f, 1f)]
        [Tooltip("How often should this feature actually appear when chosen by the generator?")]
        private float baseFrequency = 0.5f;

        [SerializeField]
        private GameObject prefab;

        [SerializeField]
        private FeatureType type = FeatureType.ADD_ON;

        [SerializeField]
        private AlignmentParameters alignment = 
            new AlignmentParameters(Vector3.zero, Vector3.zero, 0, false);

        [SerializeField]
        [Tooltip("May this feature stack with other of the same type?")]
        private bool stackable = false;

        [SerializeField]
        [Tooltip("What kind of features may be on the same cell as this one?")]
        private List<DungeonFeature> stackWhiteList = new List<DungeonFeature>();

        [SerializeField]
        private List<PerimeterRestriction> restrictions;

        #endregion

        #region public Properties

        public GameObject Prefab { get { return this.prefab; } }

        public FeatureType Type { get { return this.type; } }

        public AlignmentParameters Alignment { get { return this.alignment; } }

        public float BaseFrequency { get { return this.baseFrequency; } }

        #endregion

        #region methods

        /// <summary>
        /// Checks whether a given feature could be stacked with this one.
        /// </summary>
        /// <typeparam name="T">Subtype of feature being checked.</typeparam>
        /// <param name="feature">Feature being checked.</param>
        /// <returns>True if this feature is allowed to stack with the given one.</returns>
        public bool MayStackWith<T>(T feature) where T : DungeonFeature
        {
            if (this.type == FeatureType.REPLACEMENT && feature.type == FeatureType.REPLACEMENT)
                return false;
            else if (!stackable && this is T)
                return false;
            else if (!stackable)
                return stackWhiteList.Contains(feature);
            return true;
        }

        public bool IsPositionValid(Dungeon d, Vector2 pos)
        {
            return !restrictions.Any(restriction =>
            {
                if (restriction.above)
                {
                    if (Check(d, pos.SumY(1), restriction))
                        return true;
                }
                if (restriction.below)
                {
                    if (Check(d, pos.SumY(-1), restriction))
                        return true;
                }
                if (restriction.left)
                {
                    if (Check(d, pos.SumX(-1), restriction))
                        return true;
                }
                if (restriction.right)
                {
                    if (Check(d, pos.SumX(1), restriction))
                        return true;
                }
                return false;
            });
        }

        private bool Check(Dungeon d, Vector2 cell, PerimeterRestriction restriction)
        {
            if (restriction.features.IsNullOrEmpty() ||
                !d.IsWithinDungeon(cell) ||
                !d[cell])
                return false;

            var features = d.GetFeaturesAt(cell);
            if (features.IsNullOrEmpty())
                return false;
            
            return (features.Intersect(restriction.features).Any());
        }

        /// <summary>
        /// Defines a range for the the possible offset (in world units) that this feature may have.
        /// </summary>
        /// <param name="minOffset">Minimum value for offset.</param>
        /// <param name="maxOffset">Maximum value for offset.</param>
        /// <param name="faceOffet">Should the prefab of this feature have it's forward vector pointing towards the direction it was offset to?</param>
        public void SetOffset(Vector3 minOffset, Vector3? maxOffset = null, int faceOffset = 0)
        {
            this.alignment.minOffset = minOffset;
            if (maxOffset == null)
                this.alignment.maxOffset = minOffset;
            else
                this.alignment.maxOffset = Vector3.Max((Vector3)maxOffset, minOffset);

            if (faceOffset != 0)
                this.alignment.faceOffset = Mathf.Clamp(faceOffset, -1, 1);
        }

        public override bool Equals(object o)
        {
            if (o == null)
                return false;
            DungeonFeature f = o as DungeonFeature;
            if (!f)
                return false;

            if (!f.GetType().Equals(this.GetType()))
                return false;

            if (f.type != this.type)
                return false;

            if (!f.name.Equals(this.name))
                return false;

            if (!f.prefab.Equals(this.prefab))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return this.type.GetHashCode() + this.name.GetHashCode() * 3 + this.prefab.GetHashCode() * 77;
        }
        #endregion
    }
}