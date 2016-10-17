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
            PURGE = 4,
            CARD = 5,
            CARDS = 6,
            IDCLIP = 7
        }

        private CheatCodes _currentCode;

        private string storedCode;
        private static readonly string[] codes = {"ISEXIT", "ISDIE", "ISWIN", "ISNEXT", "ISPURGE", "ISCARD", "ISXCARDS%n", "IDCLIP"};
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

                print(lastFrameInput);
                print(storedCode);

                int codeToExecute = 0;
                bool inputMatchesAny = false;

                while (codeToExecute < codes.Length)
                {
                    string code = codes[codeToExecute];
                    if (code.Equals(storedCode))
                    {
                        print("EXECUTING CHEAT CODE N" + codeToExecute + ": " + storedCode);
                        _currentCode = (CheatCodes)codeToExecute;
                        CheckExecutedCommand();
                        return;
                    }
                    else
                    {
                        int val;
                        if (int.TryParse(lastFrameInput, out val))
                        {
                            code = code.Replace("%n", lastFrameInput);
                            Log.Info(this, codeToExecute == 6, "Parsed input to obtain code {0}, now comparing with code stored {1}", code, storedCode);
                            if (code.Equals(storedCode))
                            {
                                print("EXECUTING CHEAT CODE N" + codeToExecute + ": " + storedCode);
                                _currentCode = (CheatCodes)codeToExecute;
                                CheckExecutedCommand(val);
                                return;
                            }
                        }
                        
                        if (codes[codeToExecute].StartsWith(storedCode))
                        {
                            return;
                        }
                        else if (codes[codeToExecute].StartsWith(lastFrameInput))
                        {
                            inputMatchesAny = true;
                        }
                    }
                    codeToExecute++;
                }

                storedCode = inputMatchesAny ? lastFrameInput : "";
            }
        }

        private void UnlockExits()
        {
            print("Open sesame");
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Exit"))
                obj.GetComponent<Exit>().Unlock();
        }

        private void KillPlayer()
        {
            print("Death comes to all...");
            if (GameManager.Instance.Player)
            {
                GameManager.Instance.Kill(GameManager.Instance.Player.gameObject);
            }
            else
            {
                Log.Warn(this, "except to a non-existing player.");
            }
        }

        private void KillEnemies()
        {
            print("Destroy my enemies... and my life is yours");
            foreach (var e in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                GameManager.Instance.Kill(e);
            }
        }

        private void SummonCard(int amount = 1)
        {
            print("Pick a card... any card");
            if (GameManager.Instance.Player)
            {
                GameObject.FindObjectOfType<CardsManager>().GivePlayerCard(amount);
            }
            else
            {
                Log.Warn(this, "is what I would ask the player, if there was one....");
            }
        }

        private void TogglePlayerCollision()
        {
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
                Log.Warn(this, "and out of player....");
            }
        }

        private void CheckExecutedCommand(params object[] parameters)
        {
            switch (_currentCode)
            {
                case CheatCodes.EXIT:
                    UnlockExits();
                    break;
                case CheatCodes.DIE:
                    KillPlayer();
                    break;
                case CheatCodes.WIN:
                    print("Victorious reign");
                    GameManager.Instance.Win();
                    break;
                case CheatCodes.NEXT:
                    print("I pass");
                    GameManager.Instance.DungeonManager.CreateDungeon();
                    break;
                case CheatCodes.PURGE:
                    KillEnemies();
                    break;
                case CheatCodes.CARD:
                    SummonCard();
                    break;
                case CheatCodes.CARDS:
                    SummonCard((int)parameters[0]);
                    break;
                case CheatCodes.IDCLIP:
                    TogglePlayerCollision();
                    break;
                default:
                    return;
            }
            this.storedCode = string.Empty;
        }
    }
}