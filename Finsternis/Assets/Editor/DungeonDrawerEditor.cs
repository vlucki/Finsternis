using UnityEngine;
using UnityEditor;

namespace Finsternis
{
    [CustomEditor(typeof(DungeonDrawer))]
    public class DungeonDrawerEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DungeonDrawer tgt = target as DungeonDrawer;
            if (tgt)
            {

                if (GUILayout.Button("Draw"))
                    tgt.Draw(FindObjectOfType<Dungeon>());
            }
        }
    }
}