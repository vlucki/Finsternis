using System;
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
        if(instance != null)
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
    
    public void LoadScene(int sceneIndex)
    {
        LoadScene(SceneManager.GetSceneAt(sceneIndex).name);
    }

    public void LoadScene(string scene)
    {
        PlayerPrefs.SetString("SceneToLoad", scene);
        SceneManager.LoadScene("loading_screen");
    }

    void OnLevelWasLoaded(int index)
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if(playerObj)
            _player = playerObj.GetComponent<Entity>();
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

    public void GameOver()
    {
        LoadScene("main_menu");
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

