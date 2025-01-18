using Mirror;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace STARTING
{
    public class ChatClient : NetworkBehaviour
    {
        public CanvasGroup chatCanvasGroup;
        public GameObject chatMessagePrefab; // ä�� �޽��� ������
        public Transform chatContentPanel;
        public TMP_InputField chatInputField;
        public ScrollRect scrollView;

        private bool _isInputFieldActive = false;
        private Coroutine _fadeCoroutine;

        private void Start()
        {
            chatInputField.interactable = false;
            StartCoroutine(FadeOutChatCanvasGroup(5f));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnInputFieldSubmit();
            }
        }

        private void ToggleInputField()
        {
            _isInputFieldActive = !_isInputFieldActive;
            chatInputField.interactable = _isInputFieldActive;
            NetworkClient.localPlayer.gameObject.GetComponent<PlayerInput>().enabled = !_isInputFieldActive;


            if (_isInputFieldActive)
            {
                chatCanvasGroup.alpha = 1f;
                chatInputField.ActivateInputField();

                if (_fadeCoroutine != null)
                {
                    StopCoroutine(_fadeCoroutine);
                    _fadeCoroutine = null;
                }
            }
            else
            {
                _fadeCoroutine = StartCoroutine(FadeOutChatCanvasGroup(5f));
            }
        }

        // ä�� �޽��� �߰�, ��ũ�� ��ġ�� �Ʒ� ����
        public void AddChatMessage(string playerName, string message)
        {
            GameObject chatMessageObject = Instantiate(chatMessagePrefab, chatContentPanel);
            ChatMessage chatMessage = chatMessageObject.GetComponent<ChatMessage>();

            chatMessage.SetMessage(playerName, message);

            Canvas.ForceUpdateCanvases();
            scrollView.verticalNormalizedPosition = 0f;
        }

        // InputField ����
        private void OnInputFieldSubmit()
        {
            if (_isInputFieldActive && !string.IsNullOrEmpty(chatInputField.text))
            {
                // Ŭ���̾�Ʈ���� ������ �޽��� ����
                CmdSendChatMessage(
                    NetworkClient.localPlayer != null ? NetworkClient.localPlayer.gameObject.name : "Anonymous",
                    chatInputField.text
                );
                chatInputField.text = "";
                ToggleInputField();
            }
            else
            {
                ToggleInputField();
            }
        }

        [Command(requiresAuthority = false)]
        public void CmdSendChatMessage(string playerName, string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            
            RpcReceiveChatMessage(playerName, message);
        }
        
        [ClientRpc]
        private void RpcReceiveChatMessage(string playerName, string message)
        {
            //ChatSupport.Instance?.AddChatMessage(playerName, message);
        }
        
        private IEnumerator FadeOutChatCanvasGroup(float duration)
        {
            float startAlpha = chatCanvasGroup.alpha;
            float targetAlpha = 0.15f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                chatCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
                yield return null;
            }
            chatCanvasGroup.alpha = targetAlpha;
        }
    }
}