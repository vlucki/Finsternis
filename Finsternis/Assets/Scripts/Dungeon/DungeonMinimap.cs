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
        private CharController player;
        private RawImage image;

        private Coroutine updateRoutine;

        private HashSet<DungeonSection> exploredSections;
        private Vector3 lastPlayerPos;

        private bool shouldUpdateRenderedMap;

        void Awake()
        {
            this.image = GetComponent<RawImage>();
            if (GameManager.Instance.Player)
                SetPlayer(GameManager.Instance.Player);
            else
                GameManager.Instance.onPlayerSpawned.AddListener(SetPlayer);
        }

        void OnEnable()
        {
            StartUpdating();
        }

        public void SetPlayer(CharController player)
        {
            GameManager.Instance.onPlayerSpawned.RemoveListener(SetPlayer);
            this.player = player;
            StartUpdating();
        }

        public void SetDungeon(Dungeon dungeon)
        {
            if (this.updateRoutine != null)
            {
                StopCoroutine(this.updateRoutine);
                this.updateRoutine = null;
            }
            this.dungeon = dungeon;
            this.exploredSections = new HashSet<DungeonSection>();
            StartUpdating();
        }


        private void StartUpdating()
        {
            if (!this.isActiveAndEnabled || !this.player || !this.dungeon)
                return;

            if (this.updateRoutine == null)
                this.updateRoutine = this.StartCoroutine(_UpdateMap());
        }

        private IEnumerator _UpdateMap()
        {
            var tex = new Texture2D(dungeon.Width, dungeon.Height, TextureFormat.RGBA32, false, false);
            var colors = new Color[dungeon.Width * dungeon.Height];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = Color.clear;
            tex.SetPixels(0, 0, tex.width, tex.height, colors);
            tex.Apply();
            this.image.texture = tex;

            var playerTransform = this.player.transform;
            Vector2 lastDungeonPos = -Vector2.one;
            DungeonSection lastSection = null;
            while (player)
            {
                var currentDungeonPos = GameManager.Instance.DungeonManager.Drawer.GetDungeonPosition(playerTransform.position);
                if (lastDungeonPos != currentDungeonPos)
                {
#if LOG_INFO
                    Log.I(this, "Player dungeon position: {0}, last dungeon position: {1}, Explored sections {2}", 
                        currentDungeonPos, lastDungeonPos, this.exploredSections.SequenceToString());
#endif
                    lastDungeonPos = currentDungeonPos;
                    var section = this.dungeon[currentDungeonPos];
                    if (section)
                    {
                        if (this.exploredSections.Add(section) || lastSection != section)
                        {
                            yield return Wait.Frame();
                            PaintTexture(lastSection, section);
                            lastSection = section;
                        }
                    }
                }
                yield return Wait.Sec(.1f);
            }
        }

        private void PaintTexture(DungeonSection lastSection, DungeonSection currentSection)
        {
            var tex = this.image.texture as Texture2D;
            if (lastSection)
                UpdateMapSection(tex, lastSection, Color.gray);
            UpdateMapSection(tex, currentSection, Color.white);
            tex.Apply();
            this.image.texture = tex;
        }

        private void UpdateMapSection(Texture2D tex, DungeonSection lastSection, Color black)
        {
            foreach (var cell in lastSection)
            {
                int col = (int)cell.x;
                int row = (int)cell.y;
                tex.SetPixel(col, this.dungeon.Height - row - 1, black);
            }
        }
    }
}