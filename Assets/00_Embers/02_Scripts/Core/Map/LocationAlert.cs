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
                
                Vector2Int currentChunkCoord = Singleton.Map.GetChunkCoord(other.transform.position);
                
                Debug.Log(currentChunkCoord.x + ", " + currentChunkCoord.y);
                
                var locationChunkInfo = Singleton.Map.chunkList.chunkSceneNames
                    .FirstOrDefault(ci => ci.sceneName == $"Chunk_{currentChunkCoord.x}_{currentChunkCoord.y}");
                
                Singleton.UI.LocationNoti(locationChunkInfo.chunkDisplayName);
                FindAnyObjectByType<IngameUIController>().MapNameChange(locationChunkInfo.chunkDisplayName);
            }
        }
        
    }
}