using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Dungeon), true)]
public class DungeonEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Dungeon tgt = target as Dungeon;
        if (tgt)
        {
            if (GUILayout.Button("Generate"))
            {
                tgt.Awake();
                tgt.Generate();
                tgt.GetComponent<SimpleDungeonDrawer>().Draw();
            }
        }
    }
}
