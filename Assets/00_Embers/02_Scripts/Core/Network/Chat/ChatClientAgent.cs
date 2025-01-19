using System;
using Mirror;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace STARTING
{
    public class ChatClientAgent : MonoBehaviour
    {
        public IngameUIController uiController;
        
        public CanvasGroup chatCanvasGroup;
        public GameObject chatMessagePrefab;
        public Transform chatContentPanel;
        public TMP_InputField chatInputField;
        public ScrollRect scrollView;

        private bool _isInputFieldActive = false;
        private CancellationTokenSource _fadeCancellationTokenSource;

        private void Start()
        {
            chatInputField.interactable = false;
            _ = StartFadeOutChatCanvasGroup(5f);
        }

        private void OnEnable()
        {
            Managers.Network.ChatServer.OnMessageRecieved += AddChatMessage;
        }

        private void OnDisable()
        {
            Managers.Network.ChatServer.OnMessageRecieved -= AddChatMessage;
        }

        public void AddChatMessage(string playerName, string message)
        {
            GameObject chatMessageObject = Instantiate(chatMessagePrefab, chatContentPanel);
            ChatMessage chatMessage = chatMessageObject.GetComponent<ChatMessage>();

            chatMessage.SetMessage(playerName, message);

            Canvas.ForceUpdateCanvases();
            scrollView.verticalNormalizedPosition = 0f;
        }

        public void OpenChat()
        {
            CancelFadeOut();
            chatCanvasGroup.alpha = 1f;
            chatInputField.interactable = true;
            chatInputField.ActivateInputField();

            uiController.LocalPlayer.GetComponent<UnityEngine.InputSystem.PlayerInput>().enabled = false;
        }

        public void SendChatMessage()
        {
            if (!string.IsNullOrEmpty(chatInputField.text))
            {
                Managers.Network.ChatServer.CmdSendChatMessage(
                    NetworkClient.localPlayer != null ? NetworkClient.localPlayer.gameObject.name : "Anonymous",
                    chatInputField.text
                );
                chatInputField.text = "";
                CloseChat();
            }
            else
            {
                CloseChat();
            }
        }
        
        public void CloseChat()
        {
            _ = StartFadeOutChatCanvasGroup(5f);
            chatInputField.interactable = false;
            
            uiController.LocalPlayer.GetComponent<UnityEngine.InputSystem.PlayerInput>().enabled = true;
        }

        private async Awaitable StartFadeOutChatCanvasGroup(float duration)
        {
            CancelFadeOut();
            _fadeCancellationTokenSource = new CancellationTokenSource();
            var token = _fadeCancellationTokenSource.Token;

            float startAlpha = chatCanvasGroup.alpha;
            float targetAlpha = 0.15f;
            float elapsed = 0f;

            try
            {
                while (elapsed < duration)
                {
                    if (_isInputFieldActive) return;

                    elapsed += Time.deltaTime;
                    chatCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
                    await Awaitable.NextFrameAsync(token);
                }

                chatCanvasGroup.alpha = targetAlpha;
            }
            catch (OperationCanceledException)
            {
                // cancel token. nothing
            }
        }

        private void CancelFadeOut()
        {
            if (_fadeCancellationTokenSource != null)
            {
                _fadeCancellationTokenSource.Cancel();
                _fadeCancellationTokenSource.Dispose();
                _fadeCancellationTokenSource = null;
            }
        }
    }
}