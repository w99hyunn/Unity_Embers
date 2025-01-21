using System;
using System.Collections.Generic;
using Michsky.UI.Reach;
using UnityEngine;

namespace STARTING
{
    public class IngameWindowManager : MonoBehaviour
    {
        [Header("Windows 예외1 PauseMenu")]
        public PauseMenuManager pauseMenuManager;

        [Header("Windows 예외2 Chat")]
        public ChatUIController chatUIController;
        
        [SerializeField]
        private List<ModalWindowManager> openUIs = new List<ModalWindowManager>();
        private bool isChatting = false;
        private bool isPause = false;

        public Action<bool> OnCursorState;

        private void Start()
        {
            UpdateCursor();
        }

        public void OpenUI(ModalWindowManager ui)
        {
            if (true == isChatting)
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
            if (true == isChatting)
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
            if (false == isChatting)
            {
                chatUIController.OpenChat();
                isChatting = true;
            }
            else if (true == isChatting)
            {
                chatUIController.SendChatMessage();
                isChatting = false;
            }
            
            Debug.Log(isChatting);

            UpdateCursor();
        }
        
        /// <summary>
        /// ESC에 이 함수를 할당하면 창이 열려있으면 먼저 닫을것이고, 채팅창이 열려있으면 채팅창을 먼저 닫는다.
        /// </summary>
        public void OpenPause()
        {
            Debug.Log("OpenPause");
            if (true == isChatting)
            {
                Debug.Log("isChatting return");
                chatUIController.CloseChat();
                isChatting = false;
                UpdateCursor();
                return;
            }
            
            if (openUIs.Count > 0)
            {
                Debug.Log("openUIs.Count > 0 return");
                CloseTopUI();
                return;
            }

            isPause = true;
            pauseMenuManager.OpenPauseMenu();
            UpdateCursor();
        }
        
        private void CloseTopUI()
        {
            Debug.Log("CloseTopUI");
            if (openUIs.Count > 0)
            {
                ModalWindowManager topUI = openUIs[openUIs.Count - 1];
                CloseUI(topUI);
            }
        }

        public void ClosePause()
        {
            isPause = false;
            pauseMenuManager.ClosePauseMenu();
            UpdateCursor();
        }

        private void UpdateCursor()
        {
            // 커서 표시 조건: UI가 열려 있거나 채팅 중일 때
            if (openUIs.Count > 0 || isChatting || isPause)
            {
                Debug.Log("UpdateCursor true");
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                OnCursorState?.Invoke(true);
            }
            else
            {
                Debug.Log("UpdateCursor false");
                // UI가 모두 닫히면 커서를 숨김
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                OnCursorState?.Invoke(false);
            }
        }
    }
}