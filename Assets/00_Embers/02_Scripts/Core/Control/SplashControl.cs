using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

namespace STARTING
{
    public class SplashControl : MonoBehaviour
    {
        public GameObject startingLogo;
        public GameObject embersLogo;

        private const string nextSceneName = "Title";
        private const string sessionSceneName = "Session";

        private AsyncOperation _asyncLoad;
        private AsyncOperation _asyncLoad2;
        private void Start()
        {
            startingLogo.SetActive(false);
            embersLogo.SetActive(false);
            LoadSceneAsync().Forget();
            SwitchCanvasAndLoadScene().Forget();
        }

        private async UniTask LoadSceneAsync()
        {
            _asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
            _asyncLoad.allowSceneActivation = false;

            _asyncLoad2 = SceneManager.LoadSceneAsync(sessionSceneName, LoadSceneMode.Additive);
            _asyncLoad2.allowSceneActivation = false;

            await _asyncLoad.ToUniTask();
        }

        private async UniTask SwitchCanvasAndLoadScene()
        {
            Cursor.visible = false;
            await UniTask.Delay(4000);
            startingLogo.SetActive(true);
            await UniTask.Delay(4000);
            embersLogo.SetActive(true);
            await UniTask.Delay(4000);

            Cursor.visible = true;
            _asyncLoad.allowSceneActivation = true;
            _asyncLoad2.allowSceneActivation = true;
        }

        /// <summary>
        /// ESC누르면 스플래시화면 스킵
        /// </summary>
        /// <param name="value"></param>
        private void OnSkip(InputValue value)
        {
            Cursor.visible = true;
            SceneManager.LoadScene(SceneDataManager.GetSceneName(nextSceneName));
            SceneManager.LoadScene(SceneDataManager.GetSceneName(sessionSceneName), LoadSceneMode.Additive);
        }
    }
}