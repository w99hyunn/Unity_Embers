using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace NOLDA
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

            _ = LoadSceneAsync();
            _ = SwitchCanvasAndLoadScene();
        }

        private async Awaitable LoadSceneAsync()
        {
            _asyncLoad = SceneManager.LoadSceneAsync(_model.NextSceneName);
            _asyncLoad.allowSceneActivation = false;

            _asyncLoad2 = SceneManager.LoadSceneAsync(_model.SessionSceneName, LoadSceneMode.Additive);
            _asyncLoad2.allowSceneActivation = false;

            await _asyncLoad;
        }

        private async Awaitable SwitchCanvasAndLoadScene()
        {
            Cursor.visible = false;
            await Awaitable.WaitForSecondsAsync(3.5f);
            _view.StartingLogo(true);
            await Awaitable.WaitForSecondsAsync(4f);
            _view.EmbersLogo(true);
            await Awaitable.WaitForSecondsAsync(4f);

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
            SceneManager.LoadScene(_model.NextSceneName);
            SceneManager.LoadScene(_model.SessionSceneName, LoadSceneMode.Additive);
        }
    }
}