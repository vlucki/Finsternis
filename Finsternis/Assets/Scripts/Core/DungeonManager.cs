using UnityEngine;
using System.Collections;

[RequireComponent(typeof(DungeonFactory), typeof(DungeonDrawer))]
public class DungeonManager : MonoBehaviour
{
    [SerializeField]
    private Dungeon _dungeon;

    [SerializeField]
    private DungeonFactory _dFactory;

    [SerializeField]
    private DungeonDrawer _drawer;

    void Awake()
    {
        if (!_dungeon || !_drawer || !_dFactory)
        {
            _dFactory = GetComponent<DungeonFactory>();
            _drawer = GetComponent<DungeonDrawer>();

            if (!_dungeon)
                _dungeon = FindObjectOfType<Dungeon>();
        }
    }

    void Start()
    {
        _dFactory.Generate();
    }
}
