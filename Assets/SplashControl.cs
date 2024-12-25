using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;

namespace STARTING
{
    public class SplashControl : MonoBehaviour
    {
        private VisualElement unityLogo;
        private VisualElement startingLogo;
        private const string splashClass = "splashLogo-show";

        async void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            unityLogo = root.Q<VisualElement>("unityLogo");
            startingLogo = root.Q<VisualElement>("startingLogo");

            await PlaySplashSequence();
        }

        private async UniTask PlaySplashSequence()
        {
            await UniTask.Delay(1000);

            unityLogo.AddToClassList(splashClass);
            await UniTask.Delay(3000);
            unityLogo.RemoveFromClassList(splashClass);

            await UniTask.Delay(1500);

            startingLogo.AddToClassList(splashClass);
            await UniTask.Delay(3000);
            startingLogo.RemoveFromClassList(splashClass);

            await UniTask.Delay(1500);
            SceneManager.LoadScene(SceneDataManager.GetSceneName("Title"));
        }
    }
}