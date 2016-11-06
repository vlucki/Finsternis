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
            ISEXIT = 0,
            ISDIE = 1,
            ISWIN = 2,
            ISNEXT = 3,
            ISPURGE = 4,
            ISCARD = 5,
            IDCLIP = 6,
            ISSKYCAM = 7
        }

        private CheatCodes currentCode;

        private string storedCode;
        private string[] codes;
        private string lastFrameInput;

        [SerializeField]
        private Camera skyCamera;

        [SerializeField]
        private CardsManager cardsManager;
        private int storedValue;

        void Awake()
        {
            codes = Enum.GetNames(typeof(CheatCodes));
        }

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

                int value = 0;
                if (int.TryParse(latestInput, out value))
                {
                    this.storedValue = 10 * Mathf.Max(this.storedValue, 0) + value;

                    return;
                }
                else
                {
                    this.storedCode += latestInput;
                    bool inputMatchesAny = false;
                    for (int codeToExecute = 0; codeToExecute < codes.Length; codeToExecute++)
                    {
                        string code = codes[codeToExecute];
                        if (code.Equals(this.storedCode))
                        {
                            print("EXECUTING CHEAT CODE #" + codeToExecute + ": " + this.storedCode);
                            this.currentCode = (CheatCodes)codeToExecute;
                            CheckExecutedCommand();
                            ResetInputs();
                            return;
                        }
                        else if (this.codes[codeToExecute].StartsWith(storedCode))
                        {
                            return;
                        }
                        else if (this.codes[codeToExecute].StartsWith(lastFrameInput))
                        {
                            inputMatchesAny = true;
                        }
                    }

                    if (inputMatchesAny)
                        this.storedCode = latestInput;
                    else
                        ResetInputs();
                }
            }
        }

        private void ResetInputs()
        {
            this.storedValue = -1;
            this.storedCode = "";
        }

        private void UnlockExits()
        {
            print("Open sesame");
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Exit"))
                obj.GetComponent<Exit>().Unlock();
        }

        private void KillPlayer()
        {
            print("The easy way out, huh?");
            if (GameManager.Instance.Player)
            {
                GameManager.Instance.Kill(GameManager.Instance.Player.gameObject);
            }
            else
            {
                Log.Warn(this, "or so it would be if there existed a player to die.");
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
            switch (amount)
            {
                case 1:
                    print("Pick a card... any card");
                    break;
                case 2:
                    print("Deuce!");
                    break;
                case 3:
                    print("Three-of-a-kind");
                    break;
                case 4:
                    print("Four-of-a-kind");
                    break;
                case 5:
                    print("FULL HOUSE");
                    break;
                case 6:
                    print("We ain't playing poker anymore, are we?");
                    break;
                case 7:
                    print("Mulligan!");
                    break;
                case 8:
                    print("Well that is just getting ridiculous...");
                    break;
                case 9:
                    print("It's over 9...!");
                    break;
            }
            if (GameManager.Instance.Player)
            {
                this.cardsManager.GivePlayerCard(amount);
            }
            else
            {
                Log.Warn(this, "is what I would say to the player, if there was one....");
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
            switch (this.currentCode)
            {
                case CheatCodes.ISEXIT:
                    UnlockExits();
                    break;
                case CheatCodes.ISDIE:
                    KillPlayer();
                    break;
                case CheatCodes.ISWIN:
                    print("Victorious reign");
                    GameManager.Instance.Win();
                    break;
                case CheatCodes.ISNEXT:
                    print("I pass");
                    GameManager.Instance.DungeonManager.CreateDungeon(
                        (this.storedValue == - 1)? null : (int?)this.storedValue);
                    break;
                case CheatCodes.ISPURGE:
                    KillEnemies();
                    break;
                case CheatCodes.ISCARD:
                    SummonCard(Mathf.Max(1, this.storedValue));
                    break;
                case CheatCodes.IDCLIP:
                    TogglePlayerCollision();
                    break;
                case CheatCodes.ISSKYCAM:
                    ToggleSkyCam();
                    break;
                default:
                    return;
            }
        }

        private void ToggleSkyCam()
        {
            this.skyCamera.gameObject.SetActive(!this.skyCamera.gameObject.activeSelf);
            if (this.skyCamera.isActiveAndEnabled)
            {
                float camY = this.storedValue > 0 ? this.storedValue : this.skyCamera.transform.position.y;
                this.skyCamera.transform.position = 
                    GameManager.Instance.DungeonManager.GetComponent<DungeonDrawer>().GetWorldPosition(
                        GameManager.Instance.DungeonManager.CurrentDungeon.GetCenter()).WithY(camY);
            }

        }
    }
}