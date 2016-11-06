namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;
    using System.Collections.Generic;

    using UnityQuery;

    [RequireComponent(typeof(DungeonFactory), typeof(DungeonDrawer))]
    public class DungeonManager : MonoBehaviour
    {
        [SerializeField]
        private DungeonFactory _dFactory;

        [SerializeField]
        private DungeonDrawer _dDrawer;

        private Dungeon _currentDungeon;

        public DungeonFactory Factory { get { return _dFactory; } }

        public DungeonDrawer Drawer { get { return _dDrawer; } }

        public Dungeon CurrentDungeon { get { return _currentDungeon; } }

        void Awake()
        {
            if (!_dDrawer || !_dFactory)
            {
                _dDrawer = GetComponent<DungeonDrawer>();
                _dFactory = GetComponent<DungeonFactory>();
            }
            _dFactory.onGenerationEnd.AddListener((dungeon) =>
            {
                GameManager.Instance.ClearedDungeons++;
                _currentDungeon = dungeon;

                dungeon.OnGoalCleared.AddListener(() =>
                {
                    if (dungeon.RemainingGoals <= 0)
                    {
                        Exit e = FindObjectOfType<Exit>();
                        e.Unlock();
                    }
                });
            });
        }

        public void CreateDungeon()
        {
            this.CreateDungeon(null);
        }

        public void CreateDungeon(int? seed)
        {
            Dungeon d = CurrentDungeon;
            if (d)
            {
                d.gameObject.DestroyNow();
            }
            _dFactory.Generate(seed);
        }
    }
}