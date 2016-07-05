using UnityEngine;
using System.Collections;
using System;

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
            _dFactory.onGenerationEnd.AddListener(() =>{ GameManager.Instance.DungeonCount++; });
            _drawer = GetComponent<DungeonDrawer>();
        }
    }

    void Start()
    {
        CreateDungeon();
    }

    private void CreateDungeon()
    {
        _dungeon = _dFactory.Generate();
        _dFactory.onGenerationEnd.AddListener(() =>
        {
            _dungeon.goalCleared.AddListener(() =>
            {
                if (_dungeon.RemainingGoals <= 0)
                {
                    CreateDungeon();
                }
            });
        });
    }
}
