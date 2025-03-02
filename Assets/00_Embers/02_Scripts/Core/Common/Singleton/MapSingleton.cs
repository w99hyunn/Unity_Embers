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

        private AudioSource _audioSource;

        // 현재 로드돼있는 청크 목록
        private Dictionary<Vector2Int, ChunkLoadState> chunkStates = new Dictionary<Vector2Int, ChunkLoadState>();
        // 현재 재생중인 BGM
        private string _currentBGMName;

        private void Awake()
        {
            TryGetComponent<AudioSource>(out _audioSource);
        }

        #region # BGM(청크별 BGM 관리)
        public async Awaitable PlayBGM(AudioClip bgmClip, float fadeDuration = 1.0f)
        {
            if (bgmClip == null) return;

            //현재 재생중인 BGM과 동일하면 계속 재생(신규 X)
            if (_currentBGMName == bgmClip.name) return;

            _currentBGMName = bgmClip.name;

            if (_audioSource.isPlaying)
            {
                await FadeOut(fadeDuration);
            }

            _audioSource.clip = bgmClip;
            _audioSource.Play();
            await FadeIn(fadeDuration);
        }

        public async Awaitable StopBGM(float fadeDuration = 1.0f)
        {
            if (_audioSource.isPlaying)
            {
                await FadeOut(fadeDuration);
                _audioSource.Stop();
                _currentBGMName = null;
            }
        }

        private async Awaitable FadeOut(float duration)
        {
            float startVolume = _audioSource.volume;
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                _audioSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
                await Awaitable.NextFrameAsync();
            }

            _audioSource.volume = 0;
        }

        private async Awaitable FadeIn(float duration)
        {
            float startVolume = _audioSource.volume;
            _audioSource.volume = 0;

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                _audioSource.volume = Mathf.Lerp(0, 1, t / duration);
                await Awaitable.NextFrameAsync();
            }

            _audioSource.volume = 1;
        }
        #endregion

        #region # Load InGame

        public void LoadInGame()
        {
            LoadInGameAsync().Forget();
        }

        private async Awaitable LoadInGameAsync()
        {
            float startTime = Time.time;

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

            float elapsedTime = Time.time - startTime;
            float remainingTime = Mathf.Max(0, 3f - elapsedTime);

            if (remainingTime > 0)
            {
                await Awaitable.WaitForSecondsAsync(remainingTime);
            }

            Singleton.UI.FadeOut();
            Singleton.UI.CloseLoading();
        }
        #endregion

        #region # Return Title

        public void ReturnTitle()
        {
            ReturnTitleCoroutine().Forget();
        }

        private async Awaitable ReturnTitleCoroutine()
        {
            StopBGM().Forget();
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
                if (inGameScene.isLoaded)
                {
                    SceneManager.MoveGameObjectToScene(NetworkClient.localPlayer.gameObject, inGameScene);
                }
            }

            // 현재 활성화된 씬 설정
            string activeSceneName = $"Chunk_{currentChunkCoord.x}_{currentChunkCoord.y}";
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(activeSceneName));

            var chunkInfo = chunkList.GetChunkInfo(activeSceneName);
            if (chunkInfo != null && chunkInfo.bgm != null)
            {
                PlayBGM(chunkInfo.bgm).Forget();
            }
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
        }

        private async Awaitable UnloadChunk(string sceneName, Vector2Int coord)
        {
            chunkStates[coord] = ChunkLoadState.UNLOADING;

            if (true == SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                var unloadOp = SceneManager.UnloadSceneAsync(sceneName);
                while (unloadOp != null && !unloadOp.isDone)
                {
                    await Awaitable.NextFrameAsync();
                }
            }

            chunkStates[coord] = ChunkLoadState.NONE;
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