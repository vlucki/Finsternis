using UnityEngine;
using UnityEditor;

namespace Finsternis
{
    [CustomEditor(typeof(DungeonFactory), true)]
    public class DungeonFactoryEditor : QuickReorder
    {
        string seed = "0";
        bool deleteExisting;

        public override void OnInspectorGUI()
        {
            DrawQuickReorderHeader();
            DungeonFactory tgt = target as DungeonFactory;
            if (tgt)
            {
                GUIStyle boxStyle = new GUIStyle(EditorStyles.helpBox);
                boxStyle.fontStyle = FontStyle.Bold;
                GUILayout.BeginVertical("Debug/Editor only parameters", boxStyle);
                GUILayout.Space(15);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Fixed seed:", GUILayout.MaxWidth(70));
                seed = GUILayout.TextField(seed, GUILayout.ExpandWidth(false), GUILayout.MinWidth(40), GUILayout.Width(40), GUILayout.MaxWidth(100));
                GUILayout.Space(10);
                GUIStyle toggleStyle = new GUIStyle(EditorStyles.toggle);
                if (deleteExisting)
                    toggleStyle.fontStyle = FontStyle.Bold;
                deleteExisting = GUILayout.Toggle(deleteExisting, "Delete Existing?", toggleStyle, GUILayout.MaxWidth(125));
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
                    }
                    else
                    {
                        seedFound = true;
                    }
                    if (deleteExisting)
                    {
                        Dungeon d = FindObjectOfType<Dungeon>();
                        if (d)
                            DestroyImmediate(d.gameObject);
                    }

                    if (seedFound)
                        tgt.Generate(dungeonSeed);
                    else
                        tgt.Generate();

                    tgt.GetComponent<DungeonDrawer>().Draw(FindObjectOfType<Dungeon>());
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.EndVertical();
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            DrawDefaultInspector();
        }
    }
}