using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(fileName = "DungeonFeature", menuName = "DungeonFeature/Generic Feature", order = 1)]
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
    private Guid _id;

    [SerializeField]
    private GameObject _prefab;

    [SerializeField]
    private FeatureType _type = FeatureType.ADD_ON;

    [SerializeField]
    private CellAlignment _alignment = CellAlignment.FLOOR;

    [SerializeField]
    private Vector3 _offset = Vector3.zero;

    public Guid Id { get { return _id; } }

    public GameObject Prefab
    {
        get { return _prefab; }
    }

    public FeatureType Type { get { return _type; } }

    public CellAlignment Alignment {
        get { return _alignment; }
        set { _alignment = value; }
    }

    public Vector3 Offset
    {
        get { return _offset; }
        set { _offset = value; }
    }

    public static DungeonFeature CreateInstance(DungeonFeature reference)
    {
        DungeonFeature feature = CreateInstance<DungeonFeature>();
        feature.Init(reference);
        return feature;
    }

    protected void Init(DungeonFeature reference)
    {
        this._offset     = reference._offset;
        this._prefab     = reference._prefab;
        this._type       = reference._type;
        this._alignment  = reference._alignment;
    }

    void OnEnable()
    {
        _id = Guid.NewGuid();
    }
}
