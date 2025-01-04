using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace STARTING
{
    public class MapManager : MonoBehaviour
    {
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

            // 네트워크 플레이어 스폰
            SpawnNetworkPlayer();

            // 맵 씬 로드
            LoadMapScene();
        }

        private void LoadMapScene()
        {
            // 플레이어 데이터에서 맵 코드 가져오기
            string mapCode = Managers.Game.playerData.MapCode;
            string mapSceneName = $"Maps_{mapCode}";

            // 맵 씬 Additive로 로드
            StartCoroutine(LoadMapSceneCoroutine(mapSceneName));
        }

        private IEnumerator LoadMapSceneCoroutine(string mapSceneName)
        {
            AsyncOperation mapLoadOperation = SceneManager.LoadSceneAsync(mapSceneName, LoadSceneMode.Additive);
            while (!mapLoadOperation.isDone)
            {
                yield return null; // 맵 로딩 대기
            }

            // 맵 로드 완료 후 추가 작업 필요 시 여기에 작성
            Debug.Log($"{mapSceneName} 로드 완료");
            Managers.UI.CloseAlert();
        }

        private void SpawnNetworkPlayer()
        {
            NetworkClient.AddPlayer();
        }
    }
    

}