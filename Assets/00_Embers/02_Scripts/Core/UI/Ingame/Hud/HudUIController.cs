using Mirror;
using UnityEngine;

namespace STARTING
{
    public class HudUIController : MonoBehaviour
    {
        public HudUIView _view;
        private GameObject _localPlayer;
        public GameObject LocalPlayer => _localPlayer;
        
        async void Start()
        {
            await PlayerBind();
            _view.ingameWindowManager.OnCursorState += CursorLockState;
        }

        private void OnDisable()
        {
            _view.ingameWindowManager.OnCursorState -= CursorLockState;
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

            Singleton.Map.ReturnTitle();
        }
        
        public void CursorLockState(bool index)
        {
            _localPlayer.GetComponent<Player>().lockCursor = index;
        }
    }
}