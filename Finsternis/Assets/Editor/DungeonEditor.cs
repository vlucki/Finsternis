using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Dungeon), true)]
public class DungeonEditor : Editor
{
    string seed = "0";
    public override void OnInspectorGUI()
    {
        Dungeon tgt = target as Dungeon;
        if (tgt)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Fixed seed:");
            seed = GUILayout.TextField(seed);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Generate"))
            {
                tgt.Awake();
                int dungeonSeed = tgt.Seed;
                if (int.TryParse(seed, out dungeonSeed))
                    tgt.Seed = dungeonSeed;
                tgt.Generate();
                tgt.GetComponent<SimpleDungeonDrawer>().Draw();
            }
        }
        DrawDefaultInspector();
    }
}
