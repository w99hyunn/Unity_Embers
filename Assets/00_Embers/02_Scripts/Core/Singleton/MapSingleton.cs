using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NOLDA
{
    public class MapSingleton : MonoBehaviour
    {
        public ChunkListSO chunkList;
        
        // 현재 로드돼있는 청크 목록
        private Dictionary<Vector2Int, ChunkLoadState> chunkStates = new Dictionary<Vector2Int, ChunkLoadState>();
        
        #region # Load InGame

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

            await UpdateChunks(Singleton.Game.playerData.Position);

            NetworkClient.Ready();
            NetworkClient.AddPlayer();
            
            await Awaitable.WaitForSecondsAsync(0.5f);
            Singleton.UI.FadeOut();
            Singleton.UI.CloseAlert();
        }
        #endregion
        
        #region # Return Title

        public void ReturnTitle()
        {
            _ = ReturnTitleCoroutine();
        }

        private async Awaitable ReturnTitleCoroutine()
        {
            string inGameScene = "InGame";
            if (SceneManager.GetSceneByName(inGameScene).isLoaded)
            {
                AsyncOperation inGameUnloadOperation = SceneManager.UnloadSceneAsync(inGameScene);
                while (!inGameUnloadOperation.isDone)
                {
                    await Awaitable.NextFrameAsync();
                }
            }

            // 지금 로드되어 있는 씬들 중, "Chunk_"로 시작하는 씬을 모두 언로드
            List<Scene> chunkScenes = new List<Scene>();
            
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(i);
                if (loadedScene.name.StartsWith("Chunk_"))
                {
                    chunkScenes.Add(loadedScene);
                }
            }
            
            foreach (var chunkScene in chunkScenes)
            {
                if (chunkScene.isLoaded)
                {
                    AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(chunkScene);
                    while (!unloadOp.isDone)
                    {
                        await Awaitable.NextFrameAsync();
                    }
                    //Debug.Log($"{chunkScene.name} 청크 씬 언로드 완료");
                }
            }

            // 타이틀 씬 로드
            string titleScene = "Title";
            AsyncOperation titleLoadOperation = SceneManager.LoadSceneAsync(titleScene, LoadSceneMode.Additive);
            while (!titleLoadOperation.isDone)
            {
                await Awaitable.NextFrameAsync();
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(titleScene));
            
            chunkStates.Clear();
            Singleton.UI.CloseAlert();
            FindAnyObjectByType<TitleUIController>().ReturnBackTitle();
        }
        #endregion
        
        #region # Chunk Loading
        public async Awaitable UpdateChunks(Vector3 playerPosition)
        {
            Vector2Int currentChunkCoord = GetChunkCoord(playerPosition);

            // playerPosition값 기준으로 필요한 청크 계산
            List<Vector2Int> chunksToLoad = new List<Vector2Int>();
            for (int x = -(Singleton.Game.LoadRange); x <= Singleton.Game.LoadRange; x++)
            {
                for (int z = -(Singleton.Game.LoadRange); z <= Singleton.Game.LoadRange; z++)
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
                        //Debug.Log($"청크 {sceneName} 는 등록되지 않아서 로드하지 않고 ChunkLoadState.LOADED처리해서 무한로드 방지");
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

            //플레이어를 InGame씬으로 이동
            if (NetworkClient.localPlayer != null)
            {
                Scene inGameScene = SceneManager.GetSceneByName("InGame");
                SceneManager.MoveGameObjectToScene(NetworkClient.localPlayer.gameObject, inGameScene);
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
            //Debug.Log($"Loaded chunk: {sceneName}");
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

            //Debug.Log($"Unloaded chunk: {sceneName}");
        }

        public Vector2Int GetChunkCoord(Vector3 position)
        {
            int x = Mathf.FloorToInt(position.x / Singleton.Game.ChunkSize);
            int z = Mathf.FloorToInt(position.z / Singleton.Game.ChunkSize);
            return new Vector2Int(x, z);
        }
        #endregion
    }
}