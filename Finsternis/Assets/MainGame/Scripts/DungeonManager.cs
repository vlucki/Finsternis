using UnityEngine;
using System.Collections;

public class DungeonManager : MonoBehaviour
{

    [SerializeField]
    private SimpleDungeon _dungeon;

    [SerializeField]
    private SimpleDungeonDrawer _drawer;

    void Awake()
    {
        if (!_dungeon || !_drawer)
        {
            GameObject dungeon = GameObject.Find("Dungeon");
            if (dungeon)
            {
                if (!_dungeon)
                    _dungeon = dungeon.GetComponent<SimpleDungeon>();
                if (!_drawer)
                    _drawer = dungeon.GetComponent<SimpleDungeonDrawer>();
            }
        }
    }

    void Start()
    {
        _dungeon.Generate();
    }
}
