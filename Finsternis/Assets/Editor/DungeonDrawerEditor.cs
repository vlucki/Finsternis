using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SimpleDungeonDrawer))]
public class DungeonGeneratorEditor : Editor {

	public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SimpleDungeonDrawer tgt = target as SimpleDungeonDrawer;
        if(tgt)
        {

            if (GUILayout.Button("Draw"))
                tgt.Draw();
            if (GUILayout.Button("Clear"))
                tgt.Clear();
        }
    }
}
