namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.UI;
    using System;
    using UnityQuery;
    using System.Collections.Generic;

    [RequireComponent(typeof(RawImage))]
    public class DungeonMinimap : MonoBehaviour
    {
        private Dungeon dungeon;
        private Character player;
        private RawImage image;

        private Coroutine drawRoutine;

        private HashSet<DungeonSection> exploredSections;
        private Vector3 lastPlayerPos;

        void Awake()
        {
            this.image = GetComponent<RawImage>();
        }

        public void SetPlayer(Character player)
        {
            this.player = player;
            this.StartCoroutine(_UpdateMap());
        }

        private IEnumerator _UpdateMap()
        {
            while (player)
            {
                var currentPlayerPos = this.player.transform.position;
                if (!lastPlayerPos.Compare(currentPlayerPos))
                {
                    lastPlayerPos = currentPlayerPos;
                    var section = this.dungeon[GameManager.Instance.DungeonManager.Drawer.GetDungeonPosition(this.player.transform.position)];
                    if (section)
                        this.exploredSections.Add(section);
                }
                yield return Wait.Sec(.1f);
            }
        }

        public void SetDungeon(Dungeon dungeon)
        {
            this.dungeon = dungeon;
            this.image.texture = new Texture2D(dungeon.Width, dungeon.Height, TextureFormat.RGBA32, false);
            this.exploredSections = new HashSet<DungeonSection>();
            StopCoroutine(drawRoutine);
            drawRoutine = StartCoroutine(_DrawMap());
        }

        private IEnumerator _DrawMap()
        {
            while (dungeon)
            {
                DrawMap();
                yield return Wait.Sec(.5f);
            }
        }

        private void DrawMap()
        {
            var tex = this.image.texture as Texture2D;
            for (int col = 0; col < this.dungeon.Width; col++)
            {
                for (int row = 0; row < this.dungeon.Height; row++)
                {
                    var color = Color.white;
                    var section = this.dungeon[col, row];
                    if (!section || !this.exploredSections.Contains(section))
                        color = Color.black;

                    tex.SetPixel(col, row, color);
                }
            }
            tex.Apply();
        }
    }
}