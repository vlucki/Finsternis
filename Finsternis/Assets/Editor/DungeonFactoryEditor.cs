using UnityEngine;
using UnityEditor;
using System;

namespace Finsternis
{
    [CustomEditor(typeof(DungeonFactory), true)]
    public class DungeonFactoryEditor : QuickReorder
    {
        [SerializeField]
        string seed;
        [SerializeField]
        bool deleteExisting = true;

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
                seed = GUILayout.TextField(seed ?? "", GUILayout.ExpandWidth(false), GUILayout.MinWidth(40), GUILayout.Width(40), GUILayout.MaxWidth(100));
                GUILayout.Space(10);
                GUIStyle toggleStyle = new GUIStyle(EditorStyles.toggle);
                if (deleteExisting)
                    toggleStyle.fontStyle = FontStyle.Bold;
                deleteExisting = GUILayout.Toggle(deleteExisting, "Delete Existing?", toggleStyle, GUILayout.MaxWidth(125));
                GUILayout.BeginVertical();
                if (GUILayout.Button("Load Seed", EditorStyles.miniButton))
                {
                    seed = PlayerPrefs.GetInt(DungeonFactory.SEED_KEY, 0).ToString();
                }
                if (GUILayout.Button("Generate", EditorStyles.miniButton))
                {
                    int dungeonSeed = 0;
                    bool seedFound = int.TryParse(seed, out dungeonSeed);

                    GenerateDungeon(seedFound ? (int?)dungeonSeed : null, tgt);
                }
                else
                if (GUILayout.Button("Generate Random", EditorStyles.miniButton))
                {
                    int dungeonSeed = UnityEngine.Random.Range(0, int.MaxValue);

                    GenerateDungeon((int?)dungeonSeed, tgt);
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.EndVertical();
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            DrawDefaultInspector();
        }

        private void GenerateDungeon(int? seed, DungeonFactory tgt)
        {
            if (deleteExisting)
            {
                Dungeon d = FindObjectOfType<Dungeon>();
                if (d)
                    DestroyImmediate(d.gameObject);
            }

            System.Collections.IEnumerator generation = tgt._Generate(seed);
            while (generation.MoveNext());

            tgt.GetComponent<DungeonDrawer>().Draw(FindObjectOfType<Dungeon>());
        }
    }
}