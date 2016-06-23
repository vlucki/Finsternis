using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [SerializeField]
    private Entity _player;

    [SerializeField]
    [Range(1, 99)]
    private int _dungeonGoal = 99;

    private int _dungeonCount;

    public static GameManager Instance { get { return instance; } }

    public Entity Player { get { return _player; } }

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
        SearchPlayer();
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
        SearchPlayer();
    }

    private void SearchPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj)
        {
            _player = playerObj.GetComponent<Entity>();
            _player.GetAttribute("hp").onValueChanged.AddListener((attribute) => { if (attribute.Value <= 0) StartCoroutine(GameOver()); });
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
        return _dungeonCount >= _dungeonGoal;
    }

    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2);
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

}

