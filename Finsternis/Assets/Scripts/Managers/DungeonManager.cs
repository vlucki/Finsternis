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
        private DungeonFactory dungeonFactory;

        [SerializeField]
        private DungeonDrawer dungeonDrawer;

        private Dungeon currentDungeon;

        public DungeonFactory Factory { get { return this.dungeonFactory; } }

        public DungeonDrawer Drawer { get { return this.dungeonDrawer; } }

        public Dungeon CurrentDungeon { get { return this.currentDungeon; } }

        void Awake()
        {
            if (!dungeonDrawer || !dungeonFactory)
            {
                dungeonDrawer = GetComponent<DungeonDrawer>();
                dungeonFactory = GetComponent<DungeonFactory>();
            }
            dungeonFactory.onGenerationEnd.AddListener((dungeon) =>
            {
                GameManager.Instance.ClearedDungeons++;
                currentDungeon = dungeon;

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
            this.Drawer.StopAllCoroutines();
            this.Factory.StopAllCoroutines();
            Dungeon d = CurrentDungeon;
            if (d)
            {
                d.gameObject.DestroyNow();
            }

            this.dungeonFactory.Generate(seed);
        }
    }
}