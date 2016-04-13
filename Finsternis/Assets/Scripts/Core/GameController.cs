using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void GoTo(string scene)
    {
        SceneManager.LoadSceneAsync(scene);
    }

    public void Exit(bool askForConfirmation = true)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

