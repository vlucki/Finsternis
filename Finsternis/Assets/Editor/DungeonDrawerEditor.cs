using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DungeonDrawer))]
public class DungeonDrawerEditor : QuickReorder {

	public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DungeonDrawer tgt = target as DungeonDrawer;
        if(tgt)
        {

            if (GUILayout.Button("Draw"))
                tgt.Draw(FindObjectOfType<Dungeon>());
        }
    }
}