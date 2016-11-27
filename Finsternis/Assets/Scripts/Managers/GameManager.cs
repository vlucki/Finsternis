namespace Finsternis
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using System;
    using UnityQuery;
    using System.Collections.Generic;
    using UnityStandardAssets.ImageEffects;

    [AddComponentMenu("Finsternis/Game Manager")]
    [DisallowMultipleComponent]
    public class GameManager : MonoBehaviour
    {
        [Serializable]
        public class PlayerSpawnedEvent : CustomEvent<CharController> { }

        [SerializeField]
        public struct Callback
        {
            public readonly UnityEngine.Object owner;
            public readonly Action<object[]> callback;
            public readonly object[] parameters;

            public Callback(UnityEngine.Object owner, Action<object[]> callback, params object[] parameters)
            {
                this.owner = owner;
                this.callback = callback;
                this.parameters = parameters;
            }
        }

        private static GameManager instance;
        private CharController player;

        #region editor variables

        [SerializeField]
        private DungeonManager dungeonManager;
        
        [SerializeField][SceneSelection]
        private string mainGameName = "DungeonGeneration";

        [SerializeField]
        private int dungeonsToClear = 4;

        public PlayerSpawnedEvent onPlayerSpawned;
        #endregion

        private Dictionary<string, List<Callback>> globalEvents;

        private List<MessageController> messagePool;

        private CardsManager cardsManager;


        public static GameManager Instance { get { return instance; } }

        public CharController Player { get { return this.player; } }

        public DungeonManager DungeonManager { get { return this.dungeonManager; } }

        public int DungeonsToClear { get { return this.dungeonsToClear; } }

        public CardsManager CardsManager
        {
            get {
                if (!this.cardsManager) this.cardsManager = FindObjectOfType<CardsManager>();
                return this.cardsManager;
            }
        }

        void Awake()
        {
            Application.targetFrameRate = 60;

            if (instance != null)
            {
                gameObject.DestroyNow();
                return;
            }
            globalEvents = new Dictionary<string, List<Callback>>();
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;

#if !UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Locked;
#endif
        }

        private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (this.dungeonManager)
                return;

            if (scene.name.Equals(this.mainGameName))
            {
                Init();
                if (!LoadSavedGame())
                {
                    Resources.FindObjectsOfTypeAll<CharacterSelectionMenu>()[0].BeginOpening();
                    CreateDungeon();
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
            this.dungeonManager = FindObjectOfType<DungeonManager>();
            
            if (this.dungeonManager)
            {
                this.dungeonManager.onDungeonCleared.AddListener(StartNextLevel);
                this.DungeonManager.Factory.onGenerationEnd.AddListener(BeginNewLevel);
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
            if (SceneManager.GetActiveScene().name.Equals(this.mainGameName))
                SaveGame();
        }

        public void SpawnPlayer(GameObject playerPrefab, Vector3 position)
        {
            this.player = ((GameObject)Instantiate(playerPrefab, position, Quaternion.identity)).GetComponent<CharController>();
            this.player.gameObject.SetActive(true);
            this.player.Character.onDeath.AddListener(() => {
                this.CallDelayed(2, GameOver);
            });
            this.CallDelayed(1, this.player.GetComponent<InputRouter>().Enable);
            this.player.Character.onAttributeInitialized.AddListener(
                attribute =>
                {
                    if (attribute.HasMaximumValue)
                        new AttributeRegeneration(
                            this.player.Character, 
                            attribute, 
                            AttributeRegeneration.RegenType.RELATIVE, .01f, .25f);
                });
            onPlayerSpawned.Invoke(this.player);
        }

        public void SpawnPlayerAtEntrance(GameObject playerPrefab)
        {
            SpawnPlayer(playerPrefab, this.dungeonManager.Drawer.GetEntrancePosition().WithY(0.5f));
        }

        private void CreateDungeon()
        {
            if(this.dungeonManager)
                this.dungeonManager.CreateDungeon();
        }

        private void GameOver()
        {
            DeleteSave();
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

        public bool GoalReached(int clearedDungeons)
        {
            return clearedDungeons >= this.dungeonsToClear;
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


        public void StartNextLevel(int clearedDungeons)
        {
            this.player.GetCachedComponent<Rigidbody>().velocity = new Vector3(0, this.player.GetCachedComponent<Rigidbody>().velocity.y, 0);
            this.player.transform.forward = -Vector3.forward;
            this.dungeonManager.CurrentDungeon.GetCachedComponent<DeathZone>().Disable();
            this.player.GetCachedComponent<Inventory>().SetAllowedCardPoints(0);
            this.player.GetCachedComponent<Inventory>().SetAllowedCardPoints(Mathf.CeilToInt(dungeonsToClear * (1 + clearedDungeons) / dungeonsToClear));
            this.CallDelayed(1, GoalReached(clearedDungeons) ? Win : (Action)this.dungeonManager.CreateDungeon);
        }

        private void BeginNewLevel(Dungeon dungeon)
        {
            if (!dungeon)
            {
                dungeon = dungeonManager.CurrentDungeon;
                if (!dungeon)
                {
#if DEBUG
                    Log.W(this, "There was no dungeon ready for a new level to begin!");
#endif
                    dungeonManager.CreateDungeon();
                }
            }

            GameObject cameraHolder = GameObject.FindGameObjectWithTag("MainCamera").transform.parent.gameObject;
            if (this.player)
            {
                
                Vector3 currOffset = this.player.transform.position - cameraHolder.transform.position;

                Vector3 pos = this.dungeonManager.Drawer.GetWorldPosition(dungeon.Entrance + Vectors.Half2).WithY(3);

                this.player.transform.position = pos;

                cameraHolder.transform.position = this.player.transform.position - currOffset;
            }
            else
            {
                cameraHolder.transform.rotation = Quaternion.Euler(90, 0, 0);
                cameraHolder.GetComponentInChildren<GlobalFog>().Dissipate(3);
                cameraHolder.transform.position = 
                    this.dungeonManager.Drawer.GetWorldPosition(
                        dungeon.GetCenter()).WithY(130);
            }

            foreach(var evt in globalEvents)
            {
                evt.Value.RemoveAll((subscriber) => !subscriber.owner);
            }
        }

        internal void NewGame()
        {
            DeleteSave();
            LoadScene("DungeonGeneration");
        }

        public void Win()
        {
            LoadScene("Victory");
        }

        private void DeleteSave()
        {

        }

        #region Events
        public void TriggerGlobalEvent(string eventName, params object[] parameters)
        {
#if LOG_INFO
            Log.I(this, "Triggering event {0}", eventName);
#endif

            List<Callback> callbacks;
            if(globalEvents.TryGetValue(eventName, out callbacks))
            {
                foreach (var callback in callbacks)
                    callback.callback(parameters);
            }
        }

        public void SubscribeToEveryEvent(UnityEngine.Object owner, Action<object[]> callback)
        {
            foreach(var evt in this.globalEvents)
            {
                SubscribeToEvent(evt.Key, owner, callback);
            }
        }

        public void SubscribeToEvent(string eventName, UnityEngine.Object owner, Action<object[]> callback)
        {
            List<Callback> callbacks;
            if (!this.globalEvents.TryGetValue(eventName, out callbacks))
            {
                this.globalEvents.Add(eventName, new List<Callback>(1));
            }
            else
            {
                foreach(var existingCallback in callbacks)
                {
                    if (existingCallback.owner.Equals(owner))
                        return;
                }
            }
            this.globalEvents[eventName].Add(new Callback(owner, callback));
        }

        public void UnsubscribeFromEvent(string eventName, UnityEngine.Object owner)
        {
            List<Callback> callbacks;
            if (this.globalEvents.TryGetValue(eventName, out callbacks))
            {
                callbacks.RemoveAll(callback => callback.owner.Equals(owner));
            }
        }
#endregion
    }
}