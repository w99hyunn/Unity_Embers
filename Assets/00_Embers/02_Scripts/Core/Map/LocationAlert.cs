using System.Linq;
using UnityEngine;

namespace STARTING
{
    public class LocationAlert : MonoBehaviour
    {
        private async Awaitable OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                await Awaitable.WaitForSecondsAsync(1f);
                
                Vector2Int currentChunkCoord = Managers.Map.GetChunkCoord(other.transform.position);
                
                var locationChunkInfo = Managers.Map.chunkList.chunkSceneNames
                    .FirstOrDefault(ci => ci.sceneName == $"Chunk_{currentChunkCoord.x}_{currentChunkCoord.y}");
                
                Managers.UI.LocationNoti(locationChunkInfo.chunkDisplayName);
                FindAnyObjectByType<IngameUIController>().MapNameChange(locationChunkInfo.chunkDisplayName);
            }
        }
        
    }
}