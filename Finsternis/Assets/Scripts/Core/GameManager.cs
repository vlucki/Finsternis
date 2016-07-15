using MovementEffects;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace Finsternis
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        [SerializeField]
        private Entity _player;

        [SerializeField]
        [Range(1, 99)]
        private int _dungeonsToClear = 9;

        private int _dungeonCount;

        [SerializeField]
        private GameObject _fallDeathZone;

        [SerializeField]
        private DungeonManager _dungeonManager;

        public GameObject playerPrefab;

        public string mainGameName = "DungeonGeneration";

        public static GameManager Instance { get { return instance; } }

        public Entity Player { get { return _player; } }

        public DungeonManager DungeonManager { get { return _dungeonManager; } }


        public int DungeonCount
        {
            get { return _dungeonCount; }
            set { _dungeonCount = Mathf.Max(0, value); }
        }

        public void IncreaseDungeonCount()
        {
            _dungeonCount++;
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


#if !UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Locked;
#endif
            _dungeonCount = -1;
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

        void OnLevelWasLoaded(int index)
        {
            if (SceneManager.sceneCount > index && index >= 0 && SceneManager.GetSceneAt(index).name.Equals(mainGameName))
                Init();
        }

        private void Init()
        {
            SearchPlayer();
            _fallDeathZone = GameObject.Find("FallDeathZone");
            _dungeonManager = FindObjectOfType<DungeonManager>();
            if (_dungeonManager)
            {
                _dungeonManager.Factory.onGenerationEnd.AddListener(BeginNewLevel);
            }
        }

        private void SearchPlayer()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj)
            {
                _player = playerObj.GetComponent<Entity>();
            }
            else if (playerPrefab)
            {
                _player = Instantiate(playerPrefab).GetComponent<Entity>();
            }

            if (_player)
            {
                _player.GetAttribute("hp").onValueChanged.AddListener(
                    (attribute) => { if (attribute.Value <= 0) Timing.RunCoroutine(_GameOver()); });
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
            return _dungeonCount >= _dungeonsToClear;
        }

        public IEnumerator<float> _GameOver()
        {
            yield return Timing.WaitForSeconds(2);
            LoadScene("GameOver");
        }

        public void Kill(GameObject obj)
        {
            Entity e = obj.GetComponent<Entity>();
            if (e)
            {
                EntityAttribute hp = e.GetAttribute("hp");
                if (hp)
                    hp.SetValue(0);
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
            _fallDeathZone.GetComponent<Collider>().enabled = false;

            _player.GetComponent<Rigidbody>().velocity = new Vector3(0, _player.GetComponent<Rigidbody>().velocity.y, 0);
            _player.transform.forward = -Vector3.forward;
            Timing.CallDelayed(1, _dungeonManager.CreateDungeon);
        }

        private void BeginNewLevel(Dungeon dungeon)
        {
            GameObject _cameraHolder = GameObject.FindGameObjectWithTag("MainCamera").transform.parent.gameObject;
            Vector3 currOffset = _player.transform.position - _cameraHolder.transform.position;
            Vector3 pos = Vector3.up * 30;
            if (dungeon)
            {
                pos.x = (int)(dungeon.Entrance.x * _dungeonManager.Drawer.cellScale.x) + _dungeonManager.Drawer.cellScale.x / 2;
                pos.z = (int)-(dungeon.Entrance.y * _dungeonManager.Drawer.cellScale.z) - _dungeonManager.Drawer.cellScale.z / 2;
            }
            _player.transform.position = pos;

            _cameraHolder.transform.position = _player.transform.position - currOffset;

            _fallDeathZone.GetComponent<Collider>().enabled = true;
        }
    }
}