using System;
using Mirror;
using UnityEngine;

namespace STARTING
{
    public class ChatUIController : MonoBehaviour
    {
        public ChatUIModel _model;
        public ChatUIView _view;
        
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
            //_model.AddMessage(playerName, msg);
            _view.AddChatMessage(playerName, msg);
        }

        public void OpenChat()
        {
            _view.ShowChat();
        }
        
        public void SendChatMessage()
        {
            if (!string.IsNullOrEmpty(_view.chatInputField.text))
            {
                _view.chatInputField.Select();
                Singleton.Network.ChatServer.CmdSendChatMessage(
                    NetworkClient.localPlayer != null ? NetworkClient.localPlayer.gameObject.name : "Anonymous",
                    _view.chatInputField.text
                );
                _view.chatInputField.text = String.Empty;
            }
            CloseChat();
        }

        public void CloseChat()
        {
            _view.HideChat();
        }
    }
}