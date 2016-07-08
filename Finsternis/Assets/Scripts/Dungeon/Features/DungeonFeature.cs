using UnityEngine;
using System.Collections;
using System;

[CreateAssetMenu(fileName = "DungeonFeature", menuName = "DungeonFeature/Generic Feature", order = 1)]
public class DungeonFeature : ScriptableObject
{
    public enum FeatureType
    {
        ADD_ON  = 0,
        FLOOR   = 1,
        WALL    = 2
    }

    public enum CellAlignment
    {
        RANDOM  = 1,
        CENTER  = 2,
        EDGE    = 3
    }

    [SerializeField]
    private Guid id;

    [SerializeField]
    private GameObject _prefab;

    [SerializeField]
    private FeatureType _type = FeatureType.ADD_ON;

    [SerializeField]
    private CellAlignment _alignment = CellAlignment.CENTER;

    [SerializeField]
    private Vector3 _offset = Vector3.zero;

    public Guid Id { get { return id; } }

    public GameObject Prefab { get { return _prefab; } }

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

    void OnEnable()
    {
        id = Guid.NewGuid();
    }
}
