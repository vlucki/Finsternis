namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;

    [CreateAssetMenu(fileName = "RoomTheme", menuName = "Finsternis/Dungeon/Themes/Room")]
    public class RoomTheme : DungeonSectionTheme
    {
        [SerializeField]
        private List<DungeonFeature> decorations = new List<DungeonFeature>();

        [SerializeField]
        private List<DungeonFeature> exits = new List<DungeonFeature>();

        public DungeonFeature GetRandomDecoration()
        {
            return GetRandomElement(this.decorations);
        }

        public DungeonFeature GetRandomExit()
        {
            return GetRandomElement(this.exits);
        }

        public bool HasDecorations()
        {
            return this.decorations.Count > 0;
        }
    }
}