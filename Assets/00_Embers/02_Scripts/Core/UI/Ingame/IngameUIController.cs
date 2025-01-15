using Michsky.UI.Reach;
using Mirror;
using UnityEngine;

namespace STARTING
{
    public class IngameUIController : MonoBehaviour
    {
        public PauseMenuManager pauseMenuManager;
        public GameObject localPlayer;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        async void Start()
        {
            await PlayerBind();
            pauseMenuManager.onPause += CursorLockState;
        }

        private void OnDisable()
        {
            pauseMenuManager.onPause -= CursorLockState;
        }

        private async Awaitable PlayerBind()
        {
            while (NetworkClient.localPlayer == null)
            {
                await Awaitable.NextFrameAsync();
            }
            
            localPlayer = NetworkClient.localPlayer.gameObject;
        }

        
        public void ReturnTitle()
        {
            localPlayer.GetComponent<Player>().CmdRemovePlayer();
            NetworkClient.NotReady();

            Managers.Map.ReturnTitle();
        }

        public void CursorLockState(bool index)
        {
            localPlayer.GetComponent<Player>().lockCursor = index;
        }
    }
}