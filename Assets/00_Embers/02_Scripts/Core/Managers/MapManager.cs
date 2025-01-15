using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace STARTING
{
    public class MapManager : MonoBehaviour
    {
        public ChunkListSO chunkList;

        [Tooltip("청크 사이즈(각 씬의 맵 사이즈)")]
        public float chunkSize = 300f;

        [Tooltip("로드할 주변 청크 수(1로 설정시 플레이어가 있는 청크 기준 1칸 주변까지")]
        public int loadRange = 1;

        // 현재 로드돼있는 청크 목록
        private Dictionary<Vector2Int, ChunkLoadState> chunkStates = new Dictionary<Vector2Int, ChunkLoadState>();


        #region ReturnTitle

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

        #endregion

        #region Load InGame

        public void LoadInGame()
        {
            _ = LoadInGameCoroutine();
        }

        private async Awaitable LoadInGameCoroutine()
        {
            await SceneManager.UnloadSceneAsync("Title");

            AsyncOperation inGameLoadOperation = SceneManager.LoadSceneAsync("InGame", LoadSceneMode.Additive);
            while (!inGameLoadOperation.isDone)
            {
                await Awaitable.NextFrameAsync();
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByName("InGame"));

            await UpdateChunks(Managers.Game.playerData.Position);

            NetworkClient.Ready();
            NetworkClient.AddPlayer();
            Managers.UI.CloseAlert();
        }
        
        /// <summary>
        /// 청크
        /// </summary>
        public async Awaitable UpdateChunks(Vector3 playerPosition)
        {
            Vector2Int currentChunkCoord = GetChunkCoord(playerPosition);

            // playerPosition값 기준으로 필요한 청크 계산
            List<Vector2Int> chunksToLoad = new List<Vector2Int>();
            for (int x = -loadRange; x <= loadRange; x++)
            {
                for (int z = -loadRange; z <= loadRange; z++)
                {
                    Vector2Int coord = new Vector2Int(currentChunkCoord.x + x, currentChunkCoord.y + z);
                    chunksToLoad.Add(coord);
                }
            }

            // 1) 필요한 청크 로드
            foreach (var coord in chunksToLoad)
            {
                if (!chunkStates.ContainsKey(coord) ||
                    chunkStates[coord] == ChunkLoadState.NONE)
                {
                    string sceneName = $"Chunk_{coord.x}_{coord.y}";

                    // **여기서 chunkList의 ChunkInfo에 sceneName이 있는지 검사**
                    bool existsInList = chunkList.chunkSceneNames.Any(ci => ci.sceneName == sceneName);
                    if (!existsInList)
                    {
                        Debug.Log($"청크 {sceneName} 는 등록되지 않아서 로드하지 않고 ChunkLoadState.LOADED처리해서 오류나지 않게함.");
                        chunkStates[coord] = ChunkLoadState.LOADED;
                        continue;
                    }

                    await LoadChunk(sceneName, coord);
                }
            }

            // 2) 필요 없는 청크 언로드
            var loadedCoords = new List<Vector2Int>(chunkStates.Keys);
            foreach (var coord in loadedCoords)
            {
                if (!chunksToLoad.Contains(coord))
                {
                    if (chunkStates[coord] == ChunkLoadState.LOADED)
                    {
                        string sceneName = $"Chunk_{coord.x}_{coord.y}";
                        await UnloadChunk(sceneName, coord);
                    }
                }
            }

            SceneManager.SetActiveScene(
                SceneManager.GetSceneByName($"Chunk_{currentChunkCoord.x}_{currentChunkCoord.y}"));
        }

        private async Awaitable LoadChunk(string sceneName, Vector2Int coord)
        {
            chunkStates[coord] = ChunkLoadState.LOADING;

            var loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!loadOp.isDone)
            {
                await Awaitable.NextFrameAsync();
            }

            chunkStates[coord] = ChunkLoadState.LOADED;
            Debug.Log($"Loaded chunk: {sceneName}");
        }

        private async Awaitable UnloadChunk(string sceneName, Vector2Int coord)
        {
            // 언로드 중
            chunkStates[coord] = ChunkLoadState.UNLOADING;

            var unloadOp = SceneManager.UnloadSceneAsync(sceneName);
            while (unloadOp != null && !unloadOp.isDone)
            {
                await Awaitable.NextFrameAsync();
            }

            //Remove 해주거나, None 상태로
            chunkStates.Remove(coord);
            //chunkStates[coord] = ChunkLoadState.None;

            Debug.Log($"Unloaded chunk: {sceneName}");
        }

        private Vector2Int GetChunkCoord(Vector3 position)
        {
            int x = Mathf.FloorToInt(position.x / chunkSize);
            int z = Mathf.FloorToInt(position.z / chunkSize);
            return new Vector2Int(x, z);
        }
        #endregion
    }
}