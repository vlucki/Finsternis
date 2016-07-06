using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DungeonDrawer))]
public class DungeonDrawerEditor : Editor {

	public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DungeonDrawer tgt = target as DungeonDrawer;
        if(tgt)
        {

            if (GUILayout.Button("Draw"))
                tgt.Draw(FindObjectOfType<Dungeon>());
        }
    }
}