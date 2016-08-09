using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using MovementEffects;

namespace Finsternis
{
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
                GameManager.Instance.DungeonCount++;
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
            Dungeon d = CurrentDungeon;
            if (d)
            {
#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlaying)
                    DestroyImmediate(d.gameObject);
                else
                    Destroy(d.gameObject);
#else
            Destroy(d.gameObject);
#endif
            }
            _dFactory.Generate();
        }
    }
}