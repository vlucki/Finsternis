namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.SceneManagement;
    
    using System;
    using UnityEngine.Events;

    public class SceneLoader : MonoBehaviour
    {

        [SerializeField]
        [SceneSelection]
        private string defaultScene;

        [SerializeField]
        private UnityEngine.UI.Image imageToFill;

        public UnityEvent OnAdditiveSceneLoaded;

        private void ValidateScene(ref string scene)
        {
            if (string.IsNullOrEmpty(scene))
                scene = defaultScene;
        }

        private void SetUp(string scene)
        {
            ValidateScene(ref scene);

            LoadingScreenController.sceneToLoad = scene;
        }

        public void Load(string scene)
        {
            SetUp(scene);
            SceneManager.LoadScene("LoadingScreen");
        }

        public void LoadAdditive(string scene)
        {
            ValidateScene(ref scene);
            StartCoroutine(WaitForSceneLoad(scene));
        }

        private IEnumerator WaitForSceneLoad(string scene)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            while (!asyncOperation.isDone)
            {
                if (this.imageToFill)
                    this.imageToFill.fillAmount = asyncOperation.progress;
                yield return null;
            }

            if (this.imageToFill)
                this.imageToFill.fillAmount = 1;

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
            OnAdditiveSceneLoaded.Invoke();
        }
    }
}