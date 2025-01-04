using Mirror;
using UnityEngine;

namespace STARTING
{
    public class IngameUI : MonoBehaviour
    {
        public GameObject localPlayer;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        async void Start()
        {
            await PlayerBind();
        }

        private async Awaitable PlayerBind()
        {
            while (NetworkClient.localPlayer == null)
            {
                await Awaitable.NextFrameAsync();
            }
            
            localPlayer = NetworkClient.localPlayer.gameObject;
        }
        
        // Update is called once per frame
        void Update()
        {

        }
    }
}
