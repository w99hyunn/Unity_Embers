using System.Collections.Generic;
using UnityEngine;

namespace STARTING
{
    public class ChatUIModel : MonoBehaviour
    {
        [SerializeField] [Header(("채팅 메세지 기록 / 추후 Output 가능"))]
        private List<(string playerName, string message)> _chatMessages
            = new List<(string playerName, string message)>();

        public IReadOnlyList<(string playerName, string message)> ChatMessages => _chatMessages;
        

        public void AddMessage(string playerName, string message)
        {
            _chatMessages.Add((playerName, message));
        }
    }
}