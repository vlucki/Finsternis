using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using UnityQuery;

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
            CARD = 5,
            IDCLIP = 6
        }

        private CheatCodes _currentCode;

        private string storedCode;
        private static readonly string[] codes = {"ISEXIT", "ISDIE", "ISWIN", "ISNEXT", "ISKILL", "ISCARD", "IDCLIP"};
        private string lastFrameInput;

        void Update()
        {
            if (SceneManager.GetActiveScene().name.Equals("DungeonGeneration"))
            {
                string latestInput = Input.inputString.ToUpper();

                if (lastFrameInput.IsNullOrEmpty() || !lastFrameInput.Equals(latestInput))
                    lastFrameInput = latestInput;
                else
                    lastFrameInput = null;

                if (lastFrameInput.IsNullOrEmpty())
                    return;


                storedCode += lastFrameInput;
                int codeToExecute = 0;
                bool inputMatchesAny = false;
                
                while (codeToExecute < codes.Length)
                {
                    if (codes[codeToExecute].Equals(storedCode))
                    {
                        print("EXECUTING CHEAT CODE N" + codeToExecute + ": " + storedCode);
                        _currentCode = (CheatCodes)codeToExecute;
                        CheckExecutedCommand();
                        return;
                    }
                    else if (codes[codeToExecute].StartsWith(storedCode))
                    {
                        return;
                    }
                    else if (codes[codeToExecute].StartsWith(lastFrameInput))
                    {
                        inputMatchesAny = true;
                    }
                    codeToExecute++;
                }

                storedCode = inputMatchesAny ? lastFrameInput : "";
            }
        }

        private void CheckExecutedCommand()
        {
            switch (_currentCode)
            {
                case CheatCodes.EXIT:
                    print("Open sesame");
                    foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Exit"))
                        obj.GetComponent<Exit>().Unlock();
                    break;
                case CheatCodes.DIE:
                    print("Death comes to all");
                    GameManager.Instance.Kill(GameManager.Instance.Player.gameObject);
                    break;
                case CheatCodes.WIN:
                    print("Victorious reign");
                    GameManager.Instance.Win();
                    break;
                case CheatCodes.NEXT:
                    print("I pass");
                    GameManager.Instance.DungeonManager.CreateDungeon();
                    break;
                case CheatCodes.KILL:
                    print("Destroy my enemies... and my life is yours");
                    foreach (var e in GameObject.FindGameObjectsWithTag("Enemy"))
                    {
                        GameManager.Instance.Kill(e);
                    }
                    break;
                case CheatCodes.CARD:
                    print("Pick a card... any card");
                    GameObject.FindObjectOfType<CardsManager>().GivePlayerCard(1);
                    break;
                case CheatCodes.IDCLIP:
                    print("Straight out of Doom!");
                    if (GameManager.Instance.Player)
                    {
                        var coll = GameManager.Instance.Player.GetComponent<Collider>();
                        if (coll)
                            coll.enabled = !coll.enabled;

                        var rbd = GameManager.Instance.Player.GetComponent<Rigidbody>();
                        if (rbd)
                            rbd.useGravity = coll.enabled;
                    }
                    else
                    {
                        print("no player found");
                    }
                    break;
                default:
                    return;
            }
        }
    }
}