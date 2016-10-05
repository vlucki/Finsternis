namespace Finsternis
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "DungeonFeature", menuName = "Finsternis/Dungeon/Features/Generic Feature", order = 0)]
    public class DungeonFeature : ScriptableObject
    {
        public enum FeatureType
        {
            ADD_ON = 0,
            REPLACEMENT = 1 //should this feature replace the tile it's on?
        }

        public enum CellAlignment //from where should the offset be computed?
        {
            CENTER = 0,
            X_Pos = 1,
            X_Neg = 2,
            Z_Pos = 3,
            Z_Neg = 4
        }

        [Serializable]
        public struct AlignmentParameters
        {
            public CellAlignment pivot;
            public Vector3 minOffset;
            public Vector3 maxOffset;
            public bool faceOffset;
            public AlignmentParameters(CellAlignment pivot, Vector3 minOffset, Vector3 maxOffset, bool faceOffset)
            {
                this.pivot = pivot;
                this.minOffset = minOffset;
                this.maxOffset = maxOffset;
                this.faceOffset = faceOffset;
            }
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
        private AlignmentParameters alignment = new AlignmentParameters(CellAlignment.CENTER, Vector3.zero, Vector3.zero, false);

        [SerializeField]
        [Tooltip("May this feature stack with other of the same type?")]
        private bool stackable = false;

        [SerializeField]
        [Tooltip("What kind of features may be on the same cell as this one?")]
        private List<DungeonFeature> stackWhiteList = new List<DungeonFeature>();
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
            else if(!stackable)
                return stackWhiteList.Contains(feature);
            return true;
        }

        /// <summary>
        /// Defines a range for the the possible offset (in world units) that this feature may have.
        /// </summary>
        /// <param name="minOffset">Minimum value for offset.</param>
        /// <param name="maxOffset">Maximum value for offset.</param>
        /// <param name="faceOffet">Should the prefab of this feature have it's forward vector pointing towards the direction it was offset to?</param>
        public void SetOffset(Vector3 minOffset, Vector3? maxOffset = null, bool? faceOffet = null)
        {
            this.alignment.minOffset = minOffset;
            if (maxOffset == null)
                this.alignment.maxOffset = minOffset;
            else
                this.alignment.maxOffset = Vector3.Max((Vector3)maxOffset, minOffset);

            if (faceOffet != null)
                this.alignment.faceOffset = (bool)faceOffet;
        }

        #endregion
    }
}