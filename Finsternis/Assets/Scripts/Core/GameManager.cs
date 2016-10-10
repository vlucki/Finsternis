namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using System;
    using UnityQuery;
    using System.Collections;
    using UnityEngine.Events;

    [AddComponentMenu("Finsternis/Game Manager")]
    [DisallowMultipleComponent]
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        [SerializeField]
        private CharController player;

        [SerializeField]
        [Range(1, 99)]
        private int dungeonsToClear = 13;

        private int clearedDungeons;

        [SerializeField]
        private DungeonManager dungeonManager;

        [SceneSelection]
        public string mainGameName = "DungeonGeneration";

        public UnityEvent OnPlayerSpawned;

        private bool isNewGame;

        public static GameManager Instance { get { return instance; } }

        public CharController Player { get { return this.player; } }

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
                gameObject.DestroyNow();
                return;
            }
            isNewGame = true;
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
            if (this.dungeonManager)
                return;

            if (scene.name.Equals(mainGameName))
            {
                Init();
                if (!LoadSavedGame())
                {
                    CreateDungeon();
                    FindObjectOfType<CharacterSelectionMenu>().BeginOpening();
                }
            }
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
            this.clearedDungeons = 0;
            this.dungeonsToClear = UnityEngine.Random.Range(1, 99);
            this.dungeonManager = FindObjectOfType<DungeonManager>();
            if (this.dungeonManager)
            {
                this.dungeonManager.Factory.onGenerationEnd.AddListener(BeginNewLevel);
            }
        }

        private bool LoadSavedGame()
        {
            return false;
            //set dungeon seed
            //generate dungeon without spawning enemies
            //load and place enemies
            //update state of interactable props (doors, chests and whatnot)
            //spawn player and set its position
        }

        private void SaveGame()
        {

        }

        void OnApplicationQuit()
        {
            if (SceneManager.GetActiveScene().name.Equals(mainGameName))
                SaveGame();
        }

        public void SpawnPlayer(GameObject playerPrefab, Vector3 position)
        {
            this.player = ((GameObject)Instantiate(playerPrefab, position, Quaternion.identity)).GetComponent<CharController>();
            this.player.Character.onDeath.AddListener(() => {
                this.CallDelayed(2, GameOver);
            });
            this.CallDelayed(1, this.player.GetComponent<InputRouter>().Enable);
            OnPlayerSpawned.Invoke();
        }

        public void SpawnPlayerAtEntrance(GameObject playerPrefab)
        {
            SpawnPlayer(playerPrefab, this.dungeonManager.GetComponent<DungeonDrawer>().GetEntrancePosition().WithY(0.5f));
        }

        private void CreateDungeon()
        {
            if(this.dungeonManager)
                this.dungeonManager.CreateDungeon();
        }

        private void GameOver()
        {
            this.clearedDungeons = 0;
            LoadScene("GameOver");
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

        public void Kill(GameObject obj)
        {
            Entity e = obj.GetComponent<Entity>();
            if (e)
            {
                EntityAttribute hp = e.GetAttribute("vit");
                if (hp)
                    hp.SetBaseValue(0);
                else
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

            this.player.GetComponent<Rigidbody>().velocity = new Vector3(0, this.player.GetComponent<Rigidbody>().velocity.y, 0);
            this.player.transform.forward = -Vector3.forward;
            clearedDungeons++;
            if (!GoalReached())
                this.CallDelayed(1, this.dungeonManager.CreateDungeon);
            else
                Win();
        }

        private void BeginNewLevel(Dungeon dungeon)
        {
            if (!dungeon)
                throw new ArgumentNullException("dungeon", "There must exist a dungeon for a new level to begin.");

            if (this.player)
            {
                GameObject cameraHolder = GameObject.FindGameObjectWithTag("MainCamera").transform.parent.gameObject;
                Vector3 currOffset = this.player.transform.position - cameraHolder.transform.position;

                Vector3 pos = this.dungeonManager.Drawer.GetWorldPosition(dungeon.Entrance + Vector2.one / 2).WithY(3);

                this.player.transform.position = pos;

                cameraHolder.transform.position = this.player.transform.position - currOffset;
            }
        }

        internal void NewGame()
        {
            this.clearedDungeons = 0;
            isNewGame = true;
            LoadScene("DungeonGeneration");
        }

        public void Win()
        {
            NewGame();
        }
    }
}