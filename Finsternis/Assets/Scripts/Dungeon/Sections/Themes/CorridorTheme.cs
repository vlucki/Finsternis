namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;

    [CreateAssetMenu(fileName = "CorridorTheme", menuName = "Finsternis/Dungeon/Themes/Corridor")]
    public class CorridorTheme : DungeonSectionTheme
    {
        [SerializeField]
        private List<DungeonFeature> traps;

        [SerializeField]
        private List<DungeonFeature> doors;

        public DungeonFeature GetRandomTrap()
        {
            return GetRandomElement(this.traps);
        }

        public DungeonFeature GetRandomDoor()
        {
            return GetRandomElement(this.doors);
        }

    }
}