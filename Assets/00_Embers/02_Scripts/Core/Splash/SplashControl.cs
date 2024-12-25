using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

namespace STARTING
{
    public class SplashControl : MonoBehaviour
    {
        public GameObject startingLogo;

        private const string nextSceneName = "Title";
        private AsyncOperation _asyncLoad;

        private void Start()
        {
            startingLogo.SetActive(false);
            SwitchCanvasAndLoadScene().Forget();
            LoadSceneAsync().Forget();
        }

        private async UniTask SwitchCanvasAndLoadScene()
        {
            Cursor.visible = false;
            await UniTask.Delay(4000);
            startingLogo.SetActive(true);
            await UniTask.Delay(4000);
            Cursor.visible = true;
            _asyncLoad.allowSceneActivation = true;
        }

        private async UniTask LoadSceneAsync()
        {
            _asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
            _asyncLoad.allowSceneActivation = false;
            await _asyncLoad.ToUniTask();
        }

        //스플래시 스킵
        private void OnSkip(InputValue value)
        {
            Cursor.visible = true;
            SceneManager.LoadScene(SceneDataManager.GetSceneName(nextSceneName));
        }
    }
}