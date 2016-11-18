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
        [Serializable]
        public class DungeonClearedEvent : CustomEvent<int> { }

        [SerializeField]
        private DungeonFactory dungeonFactory;

        [SerializeField]
        private DungeonDrawer dungeonDrawer;

        public DungeonClearedEvent onDungeonCleared;

        private int dungeonsCleared;

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
                        var exits = GameObject.FindGameObjectsWithTag("Exit");
                        foreach (var eGO in exits)
                        {
                            var e = eGO.GetComponent<Exit>();
                            e.Unlock();
                            e.onExitCrossed.AddListener(exit => DungeonCleared());
                        }
                    }
                });
            });
        }

        private void DungeonCleared()
        {
            this.dungeonsCleared++;
            this.onDungeonCleared.Invoke(this.dungeonsCleared);
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