namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;
    [CreateAssetMenu(fileName = "RoomTheme", menuName = "Finsternis/Dungeon/Themes/Room Theme")]
    public class RoomTheme : DungeonSectionTheme
    {
        [SerializeField]
        private List<GameObject> decorationPrefabs;

        public GameObject GetRandomDecoration()
        {
            return this.decorationPrefabs;
        }
    }
}