using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private KeyCode[] _exitCode = { KeyCode.E, KeyCode.X, KeyCode.I, KeyCode.T };

    private int _currentCodeLetter;

    private SimpleDungeon _dungeon;
    private SimpleDungeonDrawer _drawer;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (SceneManager.GetActiveScene().name.Equals("test"))
        {
            GameObject dungeon = GameObject.Find("Dungeon");
            if (dungeon)
            {
                _dungeon = dungeon.GetComponent<SimpleDungeon>();
                _drawer = dungeon.GetComponent<SimpleDungeonDrawer>();
            }
        }
    }

    public void GoTo(string scene)
    {
        SceneManager.LoadSceneAsync(scene);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void Update()
    {
        if(_currentCodeLetter >= _exitCode.Length)
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Exit"))
            {
                obj.GetComponent<Exit>().Unlock();
                _currentCodeLetter = 0;
            }
        }
        else if(Input.anyKeyDown)
        {
            if (Input.GetKeyDown(_exitCode[_currentCodeLetter]))
                _currentCodeLetter++;
            else
                _currentCodeLetter = 0;
        } 
    }


}

