using Mirror;
using UnityEngine;

namespace STARTING
{
    public class ChatUIController : MonoBehaviour
    {
        public ChatUIModel _model;
        public ChatUIView _view;
        
        public HudUIController hudUiController;
        
        private void OnEnable()
        {
            Singleton.Network.ChatServer.OnMessageRecieved += HandleMessageRecieved;
        }

        private void OnDisable()
        {
            Singleton.Network.ChatServer.OnMessageRecieved -= HandleMessageRecieved;
        }

        private void HandleMessageRecieved(string playerName, string msg)
        {
            _model.AddMessage(playerName, msg);
            _view.AddChatMessage(playerName, msg);
        }

        public void OpenChat()
        {
            Debug.Log("Open Chat    ");
            _view.ShowChat();
            hudUiController.LocalPlayer.GetComponent<UnityEngine.InputSystem.PlayerInput>().enabled = false;
        }
        
        public void SendChatMessage()
        {
            if (!string.IsNullOrEmpty(_view.chatInputField.text))
            {
                Singleton.Network.ChatServer.CmdSendChatMessage(
                    NetworkClient.localPlayer != null ? NetworkClient.localPlayer.gameObject.name : "Anonymous",
                    _view.chatInputField.text
                );
                _view.chatInputField.text = "";
            }
            CloseChat();
        }

        public void CloseChat()
        {
            _view.HideChat();
            hudUiController.LocalPlayer.GetComponent<UnityEngine.InputSystem.PlayerInput>().enabled = true;
        }
    }
}