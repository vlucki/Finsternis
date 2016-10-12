using UnityEngine;
using System.Collections;
using Finsternis;
using UnityEditor;

public class HelperMenus
{
    [MenuItem("GameObject/Create Game Manager", priority = 0)]
    private static void MakeManager()
    {
        var gameManager = GameObject.FindGameObjectWithTag("GameController");
        if (!gameManager)
        {
            gameManager = new GameObject("GameManager", typeof(GameManager));
            gameManager.tag = "GameController";
        }

        Selection.activeObject = gameManager;
    }
}
