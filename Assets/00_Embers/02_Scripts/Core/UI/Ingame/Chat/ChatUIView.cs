using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace NOLDA
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ChatUIView : MonoBehaviour
    {
        [Header("UI Elements")]
        public GameObject chatMessagePrefab;
        public Transform chatContentPanel;
        public InputField chatInputField;
        public ScrollRect scrollView;
        
        private CanvasGroup _chatCanvasGroup;
        private CanvasGroup _inputFieldCanvasGroup;
        
        //Chat Msg ObjectPool
        private ObjectPool<GameObject> _chatMessagePool;
        private int _currentMessages = 0;
        
        private CancellationTokenSource _fadeCancellationTokenSource;

        private void Awake()
        {
            TryGetComponent<CanvasGroup>(out _chatCanvasGroup);
            chatInputField.TryGetComponent<CanvasGroup>(out _inputFieldCanvasGroup);
            _inputFieldCanvasGroup.alpha = 0;
            
            _chatMessagePool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(chatMessagePrefab, chatContentPanel),
                actionOnGet: obj =>
                {
                    obj.SetActive(true);
                    obj.transform.SetAsLastSibling();
                },
                actionOnRelease: obj =>
                {
                    obj.SetActive(false);
                    obj.transform.SetAsFirstSibling();
                },
                actionOnDestroy: obj => Destroy(obj),
                collectionCheck: false, //중복반환
                defaultCapacity: Director.Game.ChatMaxMessages,
                maxSize: Director.Game.ChatMaxMessages
                );
        }

        private void Start()
        {
            chatInputField.interactable = false;
            _ = StartFadeOutChatCanvasGroup(5f);
        }

        public void AddChatMessage(string playerName, string message)
        {
            GameObject chatMessageObject = _chatMessagePool.Get();
            ChatMessage chatMessage = chatMessageObject.GetComponent<ChatMessage>();
            chatMessage.SetMessage(playerName, message);

            Canvas.ForceUpdateCanvases();
            scrollView.verticalNormalizedPosition = 0f;

            _currentMessages++;

            if (_currentMessages > Director.Game.ChatMaxMessages)
            {
                Transform oldestChatMessage = chatContentPanel.GetChild(0);
                _chatMessagePool.Release(oldestChatMessage.gameObject);
                _currentMessages--;
            }
        }

        public async Awaitable ShowChat()
        {
            CancelFadeOut();
            _inputFieldCanvasGroup.alpha = 1f;
            _chatCanvasGroup.alpha = 1f;
            chatInputField.interactable = true;
            await Awaitable.NextFrameAsync();
            chatInputField.ActivateInputField();
        }

        public void HideChat()
        {
            _inputFieldCanvasGroup.alpha = 0f;
            _ = StartFadeOutChatCanvasGroup(5f);
            chatInputField.interactable = false;
            chatInputField.DeactivateInputField();
        }

        private async Awaitable StartFadeOutChatCanvasGroup(float duration)
        {
            CancelFadeOut();
            _fadeCancellationTokenSource = new CancellationTokenSource();
            var token = _fadeCancellationTokenSource.Token;

            float startAlpha = _chatCanvasGroup.alpha;
            float targetAlpha = 0.15f;
            float elapsed = 0f;

            try
            {
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    _chatCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
                    await Awaitable.NextFrameAsync(token);
                }

                _chatCanvasGroup.alpha = targetAlpha;
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