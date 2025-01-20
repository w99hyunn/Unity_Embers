using System;
using System.Collections.Generic;
using Michsky.UI.Reach;
using UnityEngine;

namespace STARTING
{
    public class WindowManager : MonoBehaviour
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

        private void CloseTopUI()
        {
            Debug.Log("CloseTopUI");
            if (openUIs.Count > 0)
            {
                ModalWindowManager topUI = openUIs[openUIs.Count - 1];
                CloseUI(topUI);
            }
        }

        public void StartChat()
        {
            if (true == isChatting)
            {
                return;
            }
            
            chatUIController.OpenChat();
            isChatting = true;
            UpdateCursor();
            
        }

        public void EndChat()
        {
            if (false == isChatting)
            {
                return;
            }
            
            chatUIController.SendChatMessage();
            isChatting = false;
            UpdateCursor();

        }

        /// <summary>
        /// ESC에 이 함수를 할당하면 창이 열려있으면 먼저 닫을것이고, 채팅창이 열려있으면 채팅창을 먼저 닫는다.
        /// </summary>
        public void OpenPause()
        {
            if (true == isChatting)
            {
                chatUIController.CloseChat();
                isChatting = false;
                UpdateCursor();
                return;
            }
            
            if (openUIs.Count > 0)
            {
                CloseTopUI();
                return;
            }

            isPause = true;
            pauseMenuManager.OpenPauseMenu();
            UpdateCursor();
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
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                OnCursorState?.Invoke(true);
            }
            else
            {
                // UI가 모두 닫히면 커서를 숨김
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                OnCursorState?.Invoke(false);
            }
        }
    }
}