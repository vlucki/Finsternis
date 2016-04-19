using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private KeyCode[][] _cheatCodes;

    private int _currentCode;
    private int _currentCodeLetter;

    [SerializeField]
    private SimpleDungeon _dungeon;

    [SerializeField]
    private SimpleDungeonDrawer _drawer;

    [SerializeField]
    private Character _player;

    [SerializeField]
    [Range(1, 99)]
    private int _dungeonGoal = 99;

    private int _dungeonCount;

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
        Cursor.lockState = CursorLockMode.Locked;
        if (SceneManager.GetActiveScene().name.Equals("tests"))
        {
            if (!_dungeon || !_drawer)
            {
                GameObject dungeon = GameObject.Find("Dungeon");
                if (dungeon)
                {
                    if (!_dungeon)
                        _dungeon = dungeon.GetComponent<SimpleDungeon>();
                    if (!_drawer)
                        _drawer = dungeon.GetComponent<SimpleDungeonDrawer>();
                }
            }

            if(!_player)
                _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();

            _dungeonCount = -1;

            _cheatCodes = new KeyCode[][] {
                new KeyCode[]{ KeyCode.E, KeyCode.X, KeyCode.I, KeyCode.T },
                new KeyCode[] { KeyCode.D, KeyCode.I, KeyCode.E },
                new KeyCode[] { KeyCode.W, KeyCode.I, KeyCode.N },
                new KeyCode[] { KeyCode.N, KeyCode.E, KeyCode.X, KeyCode.T }
            };
        }

    }

    void Start()
    {
        if(_dungeon)
            _dungeon.Generate();
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
        if (SceneManager.GetActiveScene().name.Equals("tests"))
        {
            if (GoalReached())
            {
                GameOver();
            }
            else
            {
                CheckCommands();
            }
        }
    }

    private void CheckExecutedCommand()
    {
        switch (_currentCode)
        {
            case 0:
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Exit"))
                {
                    obj.GetComponent<Exit>().Unlock();
                }
                _currentCodeLetter = 0;
                break;
            case 1:
                ((RangedValueAttribute)_player.Attributes["hp"]).SetValue(0);
                _currentCodeLetter = 0;
                break;
            case 2:
                DungeonCount = _dungeonGoal + 1;
                _currentCodeLetter = 0;
                break;
            case 3:
                _dungeon.Generate();
                _currentCodeLetter = 0;
                break;
        }
    }

    private void CheckCommands()
    {
        if (_currentCodeLetter >= _cheatCodes[_currentCode].Length)
        {
            CheckExecutedCommand();
        }
        else if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(_cheatCodes[_currentCode][_currentCodeLetter]))
                _currentCodeLetter++;
            else
            {
                _currentCodeLetter = 0;
                for (int i = 0; i < _cheatCodes.Length; i++)
                {
                    if (Input.GetKeyDown(_cheatCodes[i][_currentCodeLetter]))
                    {
                        _currentCodeLetter++;
                        _currentCode = i;
                        break;
                    }
                }
            }
        }
    }

    private bool GoalReached()
    {
        return _dungeonCount >= _dungeonGoal;
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        if (controller)
        {
            ((RangedValueAttribute)controller.GetComponent<Character>().Attributes["hp"]).SetValue(0);
        }
        else
        {
            Destroy(other.gameObject);
        }
    }

    public void GameOver()
    {
        GoTo("main_menu");
    }

}

