using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class CheatsManager : MonoBehaviour
{

    private KeyCode[][] _cheatCodes;

    private int _currentCode;
    private int _currentCodeLetter;

    void Awake()
    {
        _cheatCodes = new KeyCode[][] {
                new KeyCode[] { KeyCode.E, KeyCode.X, KeyCode.I, KeyCode.T },
                new KeyCode[] { KeyCode.D, KeyCode.I, KeyCode.E },
                new KeyCode[] { KeyCode.W, KeyCode.I, KeyCode.N },
                new KeyCode[] { KeyCode.N, KeyCode.E, KeyCode.X, KeyCode.T },
                new KeyCode[] { KeyCode.K, KeyCode.I, KeyCode.L, KeyCode.L }
            };
    }

    void Update()
    {
        if (!SceneManager.GetActiveScene().name.Equals("main_menu"))
        {
            CheckCommands();
        }
    }

    private void CheckExecutedCommand()
    {
        switch (_currentCode)
        {
            case 0:
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Exit"))
                {
                    try
                    {
                        obj.GetComponent<Exit>().Unlock();
                    }
                    catch (NullReferenceException ex)
                    {
                        Debug.LogError(obj);
                        throw ex;
                    }
                }
                _currentCodeLetter = 0;
                break;
            case 1:
                GameManager.Instance.Kill(GameManager.Instance.Player.gameObject);
                _currentCodeLetter = 0;
                break;
            case 2:
                while (GameManager.Instance.GoalReached())
                    GameManager.Instance.IncreaseDungeonCount();
                _currentCodeLetter = 0;
                break;
            case 3:
                FindObjectOfType<Dungeon>().Generate();
                _currentCodeLetter = 0;
                break;
            case 4:
                foreach (var e in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    GameManager.Instance.Kill(e);
                }
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
}
