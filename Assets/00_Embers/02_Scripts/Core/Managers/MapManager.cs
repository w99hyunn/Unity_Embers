using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace STARTING
{
    public class MapManager : MonoBehaviour
    {

        public void ReturnTitle()
        {
            // 타이틀 로드를 위한 코루틴 시작
            StartCoroutine(ReturnTitleCoroutine());
        }

        private IEnumerator ReturnTitleCoroutine()
        {
            // 현재 활성화된 InGame 씬 확인
            string inGameScene = "InGame";
            if (SceneManager.GetSceneByName(inGameScene).isLoaded)
            {
                // InGame 씬 언로드
                AsyncOperation inGameUnloadOperation = SceneManager.UnloadSceneAsync(inGameScene);
                while (!inGameUnloadOperation.isDone)
                {
                    yield return null; // 기다리는 동안 대기
                }
            }

            // Managers.Game.playerData.MapCode를 사용하여 로드된 맵 씬 확인 및 언로드
            string mapCode = Managers.Game.playerData.MapCode;
            string mapSceneName = $"Maps_{mapCode}";

            if (SceneManager.GetSceneByName(mapSceneName).isLoaded)
            {
                AsyncOperation mapUnloadOperation = SceneManager.UnloadSceneAsync(mapSceneName);
                while (!mapUnloadOperation.isDone)
                {
                    yield return null; // 맵 언로드 대기
                }
                Debug.Log($"{mapSceneName} 씬 언로드 완료");
            }

            // 타이틀 씬 로드
            string titleScene = "Title";
            AsyncOperation titleLoadOperation = SceneManager.LoadSceneAsync(titleScene, LoadSceneMode.Additive);
            while (!titleLoadOperation.isDone)
            {
                yield return null; // 로딩 대기
            }

            // 타이틀 씬 활성화
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(titleScene));

            // 필요한 UI 처리 완료
            Managers.UI.CloseAlert();

            FindAnyObjectByType<TitleUIController>().ReturnBackTitle();
        }
        
        public void LoadInGame()
        {
            // Title 씬 언로드 및 InGame 씬 로드 시작
            StartCoroutine(LoadInGameCoroutine());
        }

        private IEnumerator LoadInGameCoroutine()
        {
            // Title 씬 언로드
            string titleScene = "Title";
            if (SceneManager.GetSceneByName(titleScene).isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(titleScene);
            }

            // InGame 씬 로드
            AsyncOperation inGameLoadOperation = SceneManager.LoadSceneAsync("InGame", LoadSceneMode.Additive);
            while (!inGameLoadOperation.isDone)
            {
                yield return null; // 로딩 대기
            }

            // InGame 씬 활성화
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("InGame"));

            // 맵 씬 로드
            LoadMapScene();
        }

        private void LoadMapScene()
        {
            // 플레이어 데이터에서 맵 코드 가져오기
            string mapCode = Managers.Game.playerData.MapCode;
            string mapSceneName = $"Maps_{mapCode}";

            // 맵 씬 Additive로 로드
            _ = LoadMapScene(mapSceneName);
        }
        
        private async Awaitable LoadMapScene(string mapSceneName)
        {
            // 맵 씬 Additive로 로드
            AsyncOperation mapLoadOperation = SceneManager.LoadSceneAsync(mapSceneName, LoadSceneMode.Additive);
            while (!mapLoadOperation.isDone)
            {
                await Awaitable.NextFrameAsync(); // 맵 로딩 대기
            }

            // 맵 로드 완료 후 씬 활성화
            Scene mapScene = SceneManager.GetSceneByName(mapSceneName);
            if (mapScene.IsValid()) // 로드된 씬이 유효한지 확인
            {
                SceneManager.SetActiveScene(mapScene); // 로드된 씬 활성화
                Debug.Log($"{mapScene.name} 씬 활성화 완료");
            }
            else
            {
                Debug.LogError($"씬 활성화 실패: {mapSceneName} 이 유효하지 않음.");
            }

            // 추가 작업
            Debug.Log($"{mapSceneName} 로드 완료");
            // 네트워크 플레이어 스폰
            SpawnNetworkPlayer();
            Managers.UI.CloseAlert();
        }

        private void SpawnNetworkPlayer()
        {
            NetworkClient.Ready();
            NetworkClient.AddPlayer();
        }
    }
    

}