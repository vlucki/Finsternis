namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;
    using System;

    [CreateAssetMenu(fileName = "CorridorTheme", menuName = "Finsternis/Dungeon/Themes/Corridor")]
    public class CorridorTheme : DungeonSectionTheme
    {
        [Serializable]
        public struct ModifiedFeature
        {
            public DungeonFeature feature;

            [Range(0.01f, 10f)]
            public float frequencyModifier;

            public ModifiedFeature(DungeonFeature feature, float frequencyModifier)
            {
                this.feature = feature;
                this.frequencyModifier = frequencyModifier;
            }
        }

        [SerializeField]
        private List<ModifiedFeature> traps;

        [SerializeField]
        private List<DungeonFeature> doors;

        public ModifiedFeature GetRandomTrap()
        {
            return GetRandomElement(this.traps);
        }

        public DungeonFeature GetRandomDoor()
        {
            return GetRandomElement(this.doors);
        }

    }
}