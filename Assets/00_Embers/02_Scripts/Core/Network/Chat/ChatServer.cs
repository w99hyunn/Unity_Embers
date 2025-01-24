using System;
using Mirror;

namespace NOLDA
{
    public class ChatServer : NetworkBehaviour
    {
        public Action<string, string> OnMessageRecieved;
        
        [Command(requiresAuthority = false)]
        public void CmdSendChatMessage(string playerName, string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            RpcReceiveChatMessage(playerName, message);
        }

        [ClientRpc]
        private void RpcReceiveChatMessage(string playerName, string message)
        {
            OnMessageRecieved?.Invoke(playerName, message);
        }
    }
}