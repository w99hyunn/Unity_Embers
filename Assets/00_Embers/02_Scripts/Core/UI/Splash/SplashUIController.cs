using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace NOLDA
{
    public class SplashUIController : MonoBehaviour
    {
        private const string NEXT_SCENE_NAME = "Title";
        private const string SESSION_SCENE_NAME = "Session";

        private SplashUIView _view;

        private AsyncOperation _asyncLoadTitle;
        private AsyncOperation _asyncLoadSession;

        private void Start()
        {
            TryGetComponent<SplashUIView>(out _view);

            LoadSceneAsync().Forget();
            SwitchCanvasAndLoadScene().Forget();
        }

        private async Awaitable LoadSceneAsync()
        {
            _asyncLoadTitle = SceneManager.LoadSceneAsync(NEXT_SCENE_NAME);
            _asyncLoadTitle.allowSceneActivation = false;

            _asyncLoadSession = SceneManager.LoadSceneAsync(SESSION_SCENE_NAME, LoadSceneMode.Additive);
            _asyncLoadSession.allowSceneActivation = false;

            await _asyncLoadTitle;
        }

        private async Awaitable SwitchCanvasAndLoadScene()
        {
            Cursor.visible = false;
            await Awaitable.WaitForSecondsAsync(4f);
            _view.StartingLogo(true);
            await Awaitable.WaitForSecondsAsync(4f);
            _view.EmbersLogo(true);
            await Awaitable.WaitForSecondsAsync(4f);

            Cursor.visible = true;
            _asyncLoadTitle.allowSceneActivation = true;
            _asyncLoadSession.allowSceneActivation = true;
        }

        /// <summary>
        /// ESC누르면 스플래시화면 스킵
        /// </summary>
        /// <param name="value"></param>
        private void OnSkip(InputValue value)
        {
            Cursor.visible = true;
            SceneManager.LoadScene(NEXT_SCENE_NAME);
            SceneManager.LoadScene(SESSION_SCENE_NAME, LoadSceneMode.Additive);
        }
    }
}