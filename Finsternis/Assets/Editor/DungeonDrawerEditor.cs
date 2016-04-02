using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DungeonDrawer))]
public class DungeonGeneratorEditor : Editor {

	public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DungeonDrawer tgt = target as DungeonDrawer;
        if(tgt)
        {

            if (GUILayout.Button("Clear"))
                tgt.Clear();
        }
    }
}
