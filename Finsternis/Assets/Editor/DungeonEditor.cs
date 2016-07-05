using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DungeonFactory), true)]
public class DungeonEditor : Editor
{
    string seed = "0";
    public override void OnInspectorGUI()
    {
        DungeonFactory tgt = target as DungeonFactory;
        if (tgt)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Fixed seed:");
            seed = GUILayout.TextField(seed);
            if (GUILayout.Button("Generate"))
            {
                bool seedFound = false;
                int dungeonSeed = 0;
                if (!int.TryParse(seed, out dungeonSeed))
                {
                    if (PlayerPrefs.HasKey(DungeonFactory.SEED_KEY))
                    {
                        dungeonSeed = PlayerPrefs.GetInt(DungeonFactory.SEED_KEY, 0);
                        seedFound = true;
                    }
                } else
                {
                    seedFound = true;
                }
                if (seedFound)
                    tgt.Generate(dungeonSeed);
                else
                    tgt.Generate();

                tgt.GetComponent<DungeonDrawer>().Draw();
            }
            GUILayout.EndHorizontal();
        }
        DrawDefaultInspector();
    }
}
