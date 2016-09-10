namespace Finsternis
{

    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using System;
    using UnityQuery;
    using System.Collections;

    [AddComponentMenu("Finsternis/Game Manager")]
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        [SerializeField]
        private Entity player;

        [SerializeField]
        [Range(1, 99)]
        private int dungeonsToClear = 13;

        private int clearedDungeons;

        [SerializeField]
        private GameObject fallDeathZone;

        [SerializeField]
        private DungeonManager dungeonManager;

        public GameObject playerPrefab;

        [SceneSelection]
        public string mainGameName = "DungeonGeneration";

        private bool hasManagerStarted;

        public static GameManager Instance { get { return instance; } }

        public Entity Player { get { return this.player; } }

        public DungeonManager DungeonManager { get { return this.dungeonManager; } }

        public int ClearedDungeons
        {
            get { return this.clearedDungeons; }
            set { this.clearedDungeons = Mathf.Max(0, value); }
        }

        void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            this.clearedDungeons = 0;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;

#if !UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Locked;
#endif
        }

        private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name.Equals(mainGameName))
                Init();
        }

        void Start()
        {
            Init();
        }

        public void LoadScene(int sceneIndex)
        {
            LoadScene(SceneManager.GetSceneAt(sceneIndex).name);
        }

        public void LoadScene(string sceneName)
        {
            LoadingScreenController.sceneToLoad = sceneName;
            SceneManager.LoadScene("LoadingScreen");
        }

        private void Init()
        {
            if (this.player)
                return;
            SearchPlayer();
            this.clearedDungeons = 0;
            this.dungeonsToClear = UnityEngine.Random.Range(1, 99);
            this.fallDeathZone = GameObject.Find("FallDeathZone");
            this.dungeonManager = FindObjectOfType<DungeonManager>();
            if (this.dungeonManager)
            {
                this.dungeonManager.Factory.onGenerationEnd.AddListener(BeginNewLevel);
                this.dungeonManager.CreateDungeon();
            }
        }

        private void SearchPlayer()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj)
            {
                this.player = playerObj.GetComponent<Entity>();
            }
            else if (playerPrefab)
            {
                this.player = Instantiate(playerPrefab).GetComponent<Entity>();
            }

            if (this.player)
            {
                this.player.GetAttribute("hp").onValueChanged.AddListener(
                    (attribute) => { if (attribute.Value <= 0) StartCoroutine(_GameOver()); });
            }
            else
            {
                Debug.LogWarning("Could not find a player in the scene.");
            }
        }

        public void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

        public bool GoalReached()
        {
            return this.clearedDungeons >= this.dungeonsToClear;
        }

        public IEnumerator _GameOver()
        {
            yield return Yields.SEC(2);
            LoadScene("GameOver");
        }

        public void Kill(GameObject obj)
        {
            Entity e = obj.GetComponent<Entity>();
            if (e)
            {
                EntityAttribute hp = e.GetAttribute("hp");
                if (hp)
                    hp.SetBaseValue(0);

                e.Kill();
                return;
            }
            else
            {
                Destroy(obj);
            }
        }


        internal void EndCurrentLevel(Exit e)
        {
            this.fallDeathZone.GetComponent<Collider>().enabled = false;

            this.player.GetComponent<Rigidbody>().velocity = new Vector3(0, this.player.GetComponent<Rigidbody>().velocity.y, 0);
            this.player.transform.forward = -Vector3.forward;
            clearedDungeons++;
            if (!GoalReached())
                StartCoroutine(CallDelayed(1, this.dungeonManager.CreateDungeon));
            else
                Win();
        }

        private IEnumerator CallDelayed(float delay, Action a)
        {
            yield return Yields.SEC(delay);
            a();
        }

        private void BeginNewLevel(Dungeon dungeon)
        {
            if (!dungeon)
                throw new ArgumentNullException("dungeon", "There must exist a dungeon for a new level to begin.");

            GameObject cameraHolder = GameObject.FindGameObjectWithTag("MainCamera").transform.parent.gameObject;
            Vector3 currOffset = this.player.transform.position - cameraHolder.transform.position;

            Vector3 pos = this.dungeonManager.Drawer.GetWorldPosition(dungeon.Entrance + Vector2.one / 2).WithY(3);

            this.player.transform.position = pos;

            cameraHolder.transform.position = this.player.transform.position - currOffset;

            this.fallDeathZone.GetComponent<Collider>().enabled = true;
        }

        internal void NewGame()
        {
            LoadScene("DungeonGeneration");
        }

        public void Win()
        {
            NewGame();
        }
    }
}