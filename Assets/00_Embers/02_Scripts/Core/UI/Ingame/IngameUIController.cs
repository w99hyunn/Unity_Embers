using Mirror;
using UnityEngine;

namespace STARTING
{
    public class IngameUIController : MonoBehaviour
    {
        public IngameUIView _view;
        private GameObject _localPlayer;
        
        async void Start()
        {
            await PlayerBind();
            _view.pauseMenuManager.onPause += CursorLockState;
        }

        private void OnDisable()
        {
            _view.pauseMenuManager.onPause -= CursorLockState;
        }

        private async Awaitable PlayerBind()
        {
            while (NetworkClient.localPlayer == null)
            {
                await Awaitable.NextFrameAsync();
            }
            
            _localPlayer = NetworkClient.localPlayer.gameObject;
        }

        public void MapNameChange(string mapName)
        {
            _view.mapName.SetText(mapName);
        }
        
        public void ReturnTitle()
        {
            _localPlayer.GetComponent<Player>().CmdRemovePlayer();
            NetworkClient.NotReady();

            Managers.Map.ReturnTitle();
        }

        public void CursorLockState(bool index)
        {
            _localPlayer.GetComponent<Player>().lockCursor = index;
        }
    }
}