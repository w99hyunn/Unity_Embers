using System.Linq;
using Mirror;
using UnityEngine;

namespace NOLDA
{
    public class LocationAlert : MonoBehaviour
    {
        private async Awaitable OnTriggerEnter(Collider other)
        {
            if (other.gameObject == NetworkClient.localPlayer.gameObject)
            {
                await Awaitable.WaitForSecondsAsync(1f);
                
                Vector2Int currentChunkCoord = Director.Map.GetChunkCoord(other.transform.position);
                
                var locationChunkInfo = Director.Map.chunkList.chunkSceneNames
                    .FirstOrDefault(ci => ci.sceneName == $"Chunk_{currentChunkCoord.x}_{currentChunkCoord.y}");
                
                FindAnyObjectByType<LocationNotificationUI>().LocationNoti(locationChunkInfo.chunkDisplayName);
            }
        }
    }
}