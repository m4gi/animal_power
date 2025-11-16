using System.Collections;
using Magi.Scripts.GameData;
using Tuns.Base;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class SceneLoaderSystem : Singleton<SceneLoaderSystem>
    {
        [SerializeField] private Canvas loadingCanvas;
        [SerializeField] private Slider progressBar;

        private bool isLoading = false;

        protected override void AwakeSingleton()
        {
            base.AwakeSingleton();
            if (progressBar != null)
                progressBar.value = 0;
            Application.targetFrameRate = 60;
            SceneManager.LoadScene(SceneConst.MainScene);
        }

        public void LoadScene(string sceneName)
        {
            if (isLoading) return;
            if (progressBar != null)
                progressBar.value = 0;
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            if (loadingCanvas != null) loadingCanvas.gameObject.SetActive(true);
            isLoading = true;
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);

                if (progressBar != null) progressBar.value = progress;
                if (operation.progress >= 0.9f)
                {
                    yield return new WaitForSeconds(0.5f);

                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
            if (progressBar != null) progressBar.value = 1;
            yield return new WaitForSeconds(0.5f);
            isLoading = false;
            if (loadingCanvas != null) loadingCanvas.gameObject.SetActive(false);
        }
    }
}