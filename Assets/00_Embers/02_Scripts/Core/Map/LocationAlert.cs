using System.Linq;
using Mirror;
using UnityEngine;

namespace NOLDA
{
    /// <summary>
    /// 각 청크 Trigger에 달려있음. MapManager를 시작시 할당받아, 현재 플레이어가 위치한 청크를 탐지함
    /// </summary>
    public class LocationAlert : MonoBehaviour
    {
        private MapManager mapManager;

        void Awake()
        {
            mapManager = FindAnyObjectByType<MapManager>();
        }

        private async Awaitable OnTriggerEnter(Collider other)
        {
            if (other.gameObject == NetworkClient.localPlayer.gameObject)
            {
                await Awaitable.WaitForSecondsAsync(1f);
                mapManager.RequireUpdateChunk(NetworkClient.localPlayer.gameObject.transform.position);
                Vector2Int currentChunkCoord = mapManager.GetChunkCoord(other.transform.position);

                var locationChunkInfo = mapManager.chunkList.chunkSceneNames
                    .FirstOrDefault(ci => ci.sceneName == $"Chunk_{currentChunkCoord.x}_{currentChunkCoord.y}");

                mapManager.LocationNotificationAlert(locationChunkInfo.chunkDisplayName);
            }
        }
    }
}