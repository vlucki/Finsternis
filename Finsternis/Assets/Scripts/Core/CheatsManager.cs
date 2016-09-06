using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

namespace Finsternis
{
    public class CheatsManager : MonoBehaviour
    {
        public enum CheatCodes
        {
            EXIT = 0,
            DIE = 1,
            WIN = 2,
            NEXT = 3,
            KILL = 4,
            CARD = 5
        }

        private KeyCode[][] _cheatCodes;

        private CheatCodes _currentCode;
        private int _currentCodeLetter;

        void Awake()
        {
            InitCodes();
        }

        private void InitCodes()
        {
            _cheatCodes = new KeyCode[][] {
                new KeyCode[] { KeyCode.E, KeyCode.X, KeyCode.I, KeyCode.T },
                new KeyCode[] { KeyCode.D, KeyCode.I, KeyCode.E },
                new KeyCode[] { KeyCode.W, KeyCode.I, KeyCode.N },
                new KeyCode[] { KeyCode.N, KeyCode.E, KeyCode.X, KeyCode.T },
                new KeyCode[] { KeyCode.K, KeyCode.I, KeyCode.L, KeyCode.L },
                new KeyCode[] { KeyCode.C, KeyCode.A, KeyCode.R, KeyCode.D }
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
                case CheatCodes.EXIT:
                    foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Exit"))
                        obj.GetComponent<Exit>().Unlock();
                    break;
                case CheatCodes.DIE:
                    GameManager.Instance.Kill(GameManager.Instance.Player.gameObject);
                    break;
                case CheatCodes.WIN:
                    GameManager.Instance.Win();
                    break;
                case CheatCodes.NEXT:
                    GameManager.Instance.DungeonManager.CreateDungeon();
                    break;
                case CheatCodes.KILL:
                    foreach (var e in GameObject.FindGameObjectsWithTag("Enemy"))
                    {
                        GameManager.Instance.Kill(e);
                    }
                    break;
                case CheatCodes.CARD:
                    GameObject.FindObjectOfType<CardsManager>().GivePlayerCard(1);
                    break;
                default:
                    return;
            }
            _currentCodeLetter = 0;
        }

        private void CheckCommands()
        {
            if (_cheatCodes == null)
                InitCodes();

            if (_currentCodeLetter >= _cheatCodes[(int)_currentCode].Length)
            {
                CheckExecutedCommand();
            }
            else if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(_cheatCodes[(int)_currentCode][_currentCodeLetter]))
                    _currentCodeLetter++;
                else
                {
                    _currentCodeLetter = 0;
                    for (int i = 0; i < _cheatCodes.Length; i++)
                    {
                        if (Input.GetKeyDown(_cheatCodes[i][_currentCodeLetter]))
                        {
                            _currentCodeLetter++;
                            _currentCode = (CheatCodes)i;
                            break;
                        }
                    }
                }
            }
        }
    }
}