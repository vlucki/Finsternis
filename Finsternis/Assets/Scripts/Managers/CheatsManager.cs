using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using UnityQuery;

namespace Finsternis
{
    public class CheatsManager : MonoBehaviour
    {
        private string storedCode;
        private string currentCode;
        private string lastCode;

        private string[] codes = {
            "ISEXIT"  ,
            "ISDIE"   ,
            "ISWIN"   ,
            "ISNEXT"  ,
            "ISPURGE" ,
            "ISCARD" ,
            "IDCLIP"  ,
            "ISSKYCAM",
            "ISRD"
        };
        private string lastFrameInput;

        [SerializeField]
        private Camera skyCamera;

        private int storedValue;

        void Start()
        {
            GameManager.Instance.DungeonManager.Factory.onGenerationEnd.AddListener(PositionSkyCam);
        }

        private void PositionSkyCam(Dungeon dungeon)
        {
            var center = GameManager.Instance.DungeonManager.Drawer.GetWorldPosition(dungeon.GetCenter() + Vectors.Half2);
            this.skyCamera.transform.position = center.WithY(150);
        }

        void Update()
        {
            string latestInput = Input.inputString.ToUpper();

            if (this.lastFrameInput.IsNullOrEmpty() || !this.lastFrameInput.Equals(latestInput))
                this.lastFrameInput = latestInput;
            else
                this.lastFrameInput = null;

            if (this.lastFrameInput.IsNullOrEmpty())
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
                        this.currentCode = this.storedCode;
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
#if LOG_INFO || LOG_WARN
            else
            {
                Log.W(this, "or so it would be if there existed a player to die.");
            }
#endif
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
                GameManager.Instance.CardsManager.GivePlayerCard(amount);
            }
#if LOG_INFO || LOG_WARN
            else
            {
                Log.W(this, "is what I would say to the player, if there was one....");
            }
#endif
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
#if LOG_INFO || LOG_WARN
            else
            {
                Log.W(this, "and out of player....");
            }
#endif
        }

        private void CheckExecutedCommand(params object[] parameters)
        {
            switch (this.currentCode)
            {
                case "ISEXIT":
                    UnlockExits();
                    break;
                case "ISDIE":
                    KillPlayer();
                    break;
                case "ISWIN":
                    print("Victorious reign");
                    GameManager.Instance.Win();
                    break;
                case "ISNEXT":
                    print("I pass");
                    GameManager.Instance.DungeonManager.CreateDungeon(
                        (this.storedValue == -1) ? null : (int?)this.storedValue);
                    break;
                case "ISPURGE":
                    KillEnemies();
                    break;
                case "ISCARD":
                    SummonCard(Mathf.Max(1, this.storedValue));
                    break;
                case "IDCLIP":
                    TogglePlayerCollision();
                    break;
                case "ISSKYCAM":
                    ToggleSkyCam();
                    break;
                case "ISRD":
                    this.currentCode = this.lastCode;
                    this.CheckExecutedCommand(parameters);
                    return;
                default:
                    return;
            }
            this.lastCode = this.currentCode;
        }

        private void ToggleSkyCam()
        {
            this.skyCamera.gameObject.SetActive(!this.skyCamera.gameObject.activeSelf);
            if (this.skyCamera.isActiveAndEnabled)
            {
                float camY = this.storedValue > 0 ? this.storedValue : this.skyCamera.transform.position.y;
                this.skyCamera.transform.position = this.skyCamera.transform.position.WithY(camY);
            }

        }
    }
}