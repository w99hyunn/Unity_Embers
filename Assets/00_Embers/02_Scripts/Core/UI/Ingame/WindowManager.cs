using System.Collections.Generic;
using Michsky.UI.Reach;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public PauseMenuManager pauseMenuManager;
    
    [SerializeField]
    private List<ModalWindowManager> openUIs = new List<ModalWindowManager>(); // 현재 열려 있는 UI 리스트
    [SerializeField]
    private bool isChatting = false; // 채팅 상태

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseTopUI();
        }

        if (Input.GetKeyDown(KeyCode.Return)) // Enter 키로 채팅창 제어
        {
            if (!isChatting)
            {
                StartChat();
            }
            else
            {
                EndChat();
            }
        }
    }

    public void OpenUI(ModalWindowManager ui)
    {
        if (!openUIs.Contains(ui))
        {
            openUIs.Add(ui);
            ui.OpenWindow();
            UpdateCursor();
        }
    }

    public void CloseUI(ModalWindowManager ui)
    {
        if (openUIs.Contains(ui))
        {
            openUIs.Remove(ui);
            ui.CloseWindow();
            UpdateCursor();
        }
    }

    private void CloseTopUI()
    {
        if (openUIs.Count > 0)
        {
            ModalWindowManager topUI = openUIs[openUIs.Count - 1];
            CloseUI(topUI);
        }
        else
        {
            pauseMenuManager.OpenPauseMenu();
        }
    }

    private void StartChat()
    {
        isChatting = true;
        UpdateCursor();
    }

    private void EndChat()
    {
        isChatting = false;
        UpdateCursor();
    }

    private void UpdateCursor()
    {
        // 커서 표시 조건: UI가 열려 있거나 채팅 중일 때
        if (openUIs.Count > 0 || isChatting)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // UI가 모두 닫히면 커서를 숨김
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}