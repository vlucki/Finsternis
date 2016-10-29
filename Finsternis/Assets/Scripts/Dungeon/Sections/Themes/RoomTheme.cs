namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;

    [CreateAssetMenu(fileName = "RoomTheme", menuName = "Finsternis/Dungeon/Themes/Room")]
    public class RoomTheme : DungeonSectionTheme
    {
        [SerializeField]
        private List<DungeonFeature> floorDecorations = new List<DungeonFeature>();

        [SerializeField]
        private List<DungeonFeature> wallDecorations = new List<DungeonFeature>();

        [SerializeField]
        private List<DungeonFeature> exits = new List<DungeonFeature>();

        [SerializeField]
        private List<DungeonFeature> chests = new List<DungeonFeature>();

        public DungeonFeature GetRandomFloorDecoration()
        {
            return GetRandomElement(this.floorDecorations);
        }

        public DungeonFeature GetRandomWallDecoration()
        {
            return GetRandomElement(this.wallDecorations);
        }

        public DungeonFeature GetRandomChest()
        {
            return GetRandomElement(this.chests);
        }

        public DungeonFeature GetRandomExit()
        {
            return GetRandomElement(this.exits);
        }

        public bool HasDecorations()
        {
            return this.floorDecorations.Count + this.wallDecorations.Count > 0;
        }
    }
}