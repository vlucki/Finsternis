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
        public struct PerimeterRestriction
        {
            [Range(0, 3)]
            public int above;
            [Range(0, 3)]
            public int below;
            [Range(0, 3)]
            public int left;
            [Range(0, 3)]
            public int right;
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
        [Tooltip("May this feature stack with other of the same type?")]
        private bool stackable = false;

        [SerializeField]
        [Tooltip("What kind of features may be on the same cell as this one?")]
        private List<DungeonFeature> stackWhiteList = new List<DungeonFeature>();

        [SerializeField]
        private List<PerimeterRestriction> restrictions;

        [SerializeField]
        private List<FeatureAlignment> alignment;

        #endregion

        #region public Properties

        public GameObject Prefab { get { return this.prefab; } }

        public FeatureType Type { get { return this.type; } }

        public List<FeatureAlignment> Alignment { get { return this.alignment; } }

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
                if (restriction.above > 0)
                {
                    for(int i = 1; i <= restriction.above; i++)
                        if (Check(d, pos.SumY(i), restriction))
                            return true;
                }
                if (restriction.below > 0)
                {
                    for (int i = 1; i <= restriction.below; i++)
                        if (Check(d, pos.SumY(-i), restriction))
                        return true;
                }
                if (restriction.left > 0)
                {
                    for (int i = 1; i <= restriction.left; i++)
                        if (Check(d, pos.SumX(-i), restriction))
                        return true;
                }
                if (restriction.right > 0)
                {
                    for (int i = 1; i <= restriction.right; i++)
                        if (Check(d, pos.SumX(i), restriction))
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