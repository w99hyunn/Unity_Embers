using System.Collections.Generic;
using Michsky.UI.Reach;
using Mirror;
using UnityEngine;

namespace NOLDA
{
    public class IngameControlManager : MonoBehaviour
    {
        [Header("Windows 예외1 PauseMenu")]
        public PauseMenuManager pauseMenuManager;

        [Header("Windows 예외2 Chat")]
        public ChatUIController chatUIController;
        
        [SerializeField]
        private List<ModalWindowManager> openUIs = new List<ModalWindowManager>();
        private bool _isChatting = false;
        private bool _isPause = false;

        private GameObject _localPlayer;
        public GameObject LocalPlayer => _localPlayer;

        async void Start()
        {
            await PlayerBind();
            UpdateCursor();
        }
        
        private async Awaitable PlayerBind()
        {
            while (NetworkClient.localPlayer == null)
            {
                await Awaitable.NextFrameAsync();
            }
            
            _localPlayer = NetworkClient.localPlayer.gameObject;
        }
        
        public void OpenUI(ModalWindowManager ui)
        {
            if (true == _isChatting)
                return;
            
            if (!openUIs.Contains(ui))
            {
                openUIs.Add(ui);
                ui.OpenWindow();
                UpdateCursor();
            }
        }

        public void CloseUI(ModalWindowManager ui)
        {
            if (true == _isChatting)
                return;
            
            if (openUIs.Contains(ui))
            {
                openUIs.Remove(ui);
                ui.CloseWindow();
                UpdateCursor();
            }
        }
        
        public void ToggleChat()
        {
            if (false == _isChatting)
            {
                chatUIController.OpenChat();
                _localPlayer.GetComponent<UnityEngine.InputSystem.PlayerInput>().enabled = false;
                _isChatting = true;
            }
            else if (true == _isChatting)
            {
                _ = chatUIController.SendChatMessage();
                _localPlayer.GetComponent<UnityEngine.InputSystem.PlayerInput>().enabled = true;
                _isChatting = false;
            }
            
            UpdateCursor();
        }
        
        /// <summary>
        /// ESC에 이 함수를 할당하면 창이 열려있으면 먼저 닫을것이고, 채팅창이 열려있으면 채팅창을 먼저 닫는다.
        /// </summary>
        public void OpenPause()
        {
            if (true == _isChatting)
            {
                chatUIController.CloseChat();
                _localPlayer.GetComponent<UnityEngine.InputSystem.PlayerInput>().enabled = true;
                _isChatting = false;
                UpdateCursor();
                return;
            }
            
            if (openUIs.Count > 0)
            {
                CloseTopUI();
                return;
            }

            _isPause = true;
            pauseMenuManager.OpenPauseMenu();
            UpdateCursor();
        }
        
        private void CloseTopUI()
        {
            if (openUIs.Count > 0)
            {
                ModalWindowManager topUI = openUIs[openUIs.Count - 1];
                CloseUI(topUI);
            }
        }

        public void ClosePause()
        {
            _isPause = false;
            pauseMenuManager.ClosePauseMenu();
            UpdateCursor();
        }

        private void UpdateCursor()
        {
            // 커서 표시 조건: UI가 열려 있거나 채팅 중일 때
            if (openUIs.Count > 0 || _isChatting || _isPause)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                _localPlayer.GetComponent<Player>().lockCursor = true;
            }
            else
            {
                // UI가 모두 닫히면 커서를 숨김
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                _localPlayer.GetComponent<Player>().lockCursor = false;
            }
        }
        
        public void ReturnTitle()
        {
            _localPlayer.GetComponent<Player>().CmdRemovePlayer();
            NetworkClient.NotReady();

            Singleton.Map.ReturnTitle();
        }
    }
}