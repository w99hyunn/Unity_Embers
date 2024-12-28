using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

namespace STARTING
{
    public class SplashUIController : MonoBehaviour
    {
        private SplashUIView _view;
        private SplashUIModel _model;

        private AsyncOperation _asyncLoad;
        private AsyncOperation _asyncLoad2;

        private void Start()
        {
            TryGetComponent<SplashUIView>(out _view);
            _model = new SplashUIModel();

            LoadSceneAsync().Forget();
            SwitchCanvasAndLoadScene().Forget();
        }

        private async UniTask LoadSceneAsync()
        {
            _asyncLoad = SceneManager.LoadSceneAsync(_model.nextSceneName);
            _asyncLoad.allowSceneActivation = false;

            _asyncLoad2 = SceneManager.LoadSceneAsync(_model.sessionSceneName, LoadSceneMode.Additive);
            _asyncLoad2.allowSceneActivation = false;

            await _asyncLoad.ToUniTask();
        }

        private async UniTask SwitchCanvasAndLoadScene()
        {
            Cursor.visible = false;
            await UniTask.Delay(4000);
            _view.StartingLogo(true);
            await UniTask.Delay(4000);
            _view.EmbersLogo(true);
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
            SceneManager.LoadScene(SceneDataManager.GetSceneName(_model.nextSceneName));
            SceneManager.LoadScene(SceneDataManager.GetSceneName(_model.sessionSceneName), LoadSceneMode.Additive);
        }
    }
}