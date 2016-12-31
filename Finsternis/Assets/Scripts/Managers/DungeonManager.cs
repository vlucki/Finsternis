namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using System;
    using System.Collections.Generic;

    using Extensions;

    [RequireComponent(typeof(DungeonFactory), typeof(DungeonDrawer))]
    public class DungeonManager : MonoBehaviour
    {
        [Serializable]
        public class DungeonClearedEvent : CustomEvent<int> { }

        [SerializeField]
        private DungeonFactory dungeonFactory;

        [SerializeField]
        private DungeonDrawer dungeonDrawer;

        public DungeonClearedEvent onDungeonCleared;

        public int DungeonsCleared { get; private set; }

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
                currentDungeon = dungeon;

                dungeon.onDungeonCleared.AddListener(() =>
                {
                    if (dungeon.RemainingGoals <= 0)
                    {
                        var exits = GameObject.FindGameObjectsWithTag("Exit");
                        foreach (var eGO in exits)
                        {
                            var e = eGO.GetComponent<Exit>();
                            e.Unlock();
                            e.onExitCrossed.AddListener(exit =>
                            {
                                this.Factory.SetRoomCount(Mathf.CeilToInt(this.Factory.TotalRooms * 1.1f));
                                ClearDungeon();
                            });
                        }
                    }
                });
            });
        }

        internal void ClearDungeon()
        {
            this.DungeonsCleared++;
            this.onDungeonCleared.Invoke(this.DungeonsCleared);
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

            this.dungeonFactory.Generate(seed, this.DungeonsCleared > 0);
        }
    }
}