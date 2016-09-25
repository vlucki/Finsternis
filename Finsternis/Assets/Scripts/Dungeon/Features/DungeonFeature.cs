namespace Finsternis
{
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

        [SerializeField]
        private GameObject prefab;

        [SerializeField]
        private FeatureType type = FeatureType.ADD_ON;

        [SerializeField]
        private CellAlignment alignment = CellAlignment.CENTER;

        [SerializeField]
        private Vector3 offset = Vector3.zero;

        [SerializeField][Tooltip("What kind of features may be on the same cell as this one?")]
        private List<DungeonFeature> stackWhiteList = new List<DungeonFeature>();

        public GameObject Prefab
        {
            get { return this.prefab; }
        }

        public FeatureType Type { get { return this.type; } }

        public CellAlignment Alignment
        {
            get { return this.alignment; }
            set { this.alignment = value; }
        }

        public Vector3 Offset
        {
            get { return this.offset; }
            set { this.offset = value; }
        }

        public bool MayStackWith<T>(T feature) where T : DungeonFeature
        {
            return stackWhiteList.Contains(feature);
        }
    }
}