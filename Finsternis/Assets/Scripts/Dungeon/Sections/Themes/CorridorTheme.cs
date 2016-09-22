namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;

    public class CorridorTheme : DungeonSectionTheme
    {
        [SerializeField]
        private List<TrapFeature> traps;

        [SerializeField]
        private List<DoorFeature> doors;

        public TrapFeature GetRandomTrap()
        {
            return GetRandomElement(this.traps);
        }

        public DoorFeature GetRandomDoor()
        {
            return GetRandomElement(this.doors);
        }

    }
}