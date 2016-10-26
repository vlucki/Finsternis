namespace Finsternis
{
    using UnityEngine;
    using System.Collections;

    public class CharacterSelectionMenu : MenuController
    {
        public void SetPlayer(GameObject prefab)
        {
            GameManager.Instance.SpawnPlayerAtEntrance(prefab);
            BeginClosing();
        }

        protected override void Awake()
        {
        }
    }
}