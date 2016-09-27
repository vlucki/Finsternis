namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;

    [CreateAssetMenu(fileName = "RoomTheme", menuName = "Finsternis/Dungeon/Themes/Room")]
    public class RoomTheme : DungeonSectionTheme
    {
        [SerializeField]
        private List<GameObject> decorations = new List<GameObject>();

        [SerializeField]
        private List<DungeonFeature> exits = new List<DungeonFeature>();

        public GameObject GetRandomDecoration()
        {
            return GetRandomElement(this.decorations);
        }

        public DungeonFeature GetRandomExit()
        {
            return GetRandomElement(this.exits);
        }
    }
}