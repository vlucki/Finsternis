using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(fileName = "DungeonFeature", menuName = "Finsternis/DungeonFeature/Generic Feature", order = 0)]
public class DungeonFeature : ScriptableObject
{
    public enum FeatureType
    {
        ADD_ON      = 0,
        REPLACEMENT = 1 //should this feature replace the tile it's on?
    }

    public enum CellAlignment
    {
        FLOOR   = 0,
        WALL    = 1
    }

    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private FeatureType type = FeatureType.ADD_ON;

    [SerializeField]
    private CellAlignment alignment = CellAlignment.FLOOR;

    [SerializeField]
    private Vector3 offset = Vector3.zero;

    public GameObject Prefab
    {
        get { return this.prefab; }
    }

    public FeatureType Type { get { return this.type; } }

    public CellAlignment Alignment {
        get { return this.alignment; }
        set { this.alignment = value; }
    }

    public Vector3 Offset
    {
        get { return this.offset; }
        set { this.offset = value; }
    }
}
