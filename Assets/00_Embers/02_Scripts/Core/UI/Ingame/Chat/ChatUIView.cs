using System;
using System.Threading;
using Michsky.UI.Reach;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace STARTING
{
    public class ChatUIView : MonoBehaviour
    {
        [Header("Hotkeys")]
        public HotkeyEvent openHotkey;
        public HotkeyEvent closeHotkey;
        
        [Header("UI Elements")]
        public CanvasGroup chatCanvasGroup;
        public GameObject chatMessagePrefab;
        public Transform chatContentPanel;
        public TMP_InputField chatInputField;
        public ScrollRect scrollView;
        
        private CancellationTokenSource _fadeCancellationTokenSource;

        private void Start()
        {
            chatInputField.interactable = false;
            _ = StartFadeOutChatCanvasGroup(5f);
        }

        public void AddChatMessage(string playerName, string message)
        {
            GameObject chatMessageObject = Instantiate(chatMessagePrefab, chatContentPanel);
            ChatMessage chatMessage = chatMessageObject.GetComponent<ChatMessage>();

            chatMessage.SetMessage(playerName, message);

            Canvas.ForceUpdateCanvases();
            scrollView.verticalNormalizedPosition = 0f;
        }

        public void ShowChat()
        {
            openHotkey.enabled = false;
            closeHotkey.enabled = true;
            CancelFadeOut();
            chatCanvasGroup.alpha = 1f;
            chatInputField.interactable = true;
            chatInputField.ActivateInputField();
        }

        public void HideChat()
        {
            openHotkey.enabled = true;
            closeHotkey.enabled = false;
            _ = StartFadeOutChatCanvasGroup(5f);
            chatInputField.interactable = false;
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