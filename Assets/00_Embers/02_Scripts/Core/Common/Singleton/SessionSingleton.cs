using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace NOLDA
{
    public class SessionSingleton : MonoBehaviour
    {

        [SerializeField] private List<GameObject> m_ServerPlayers = new List<GameObject>();
        public IReadOnlyList<GameObject> ServerPlayers => m_ServerPlayers;


        [Server]
        public void AddPlayer(GameObject player)
        {
            if (!m_ServerPlayers.Contains(player))
            {
                m_ServerPlayers.Add(player);
                DebugUtils.Log($"Session: '{player.name}' added. Total players: {m_ServerPlayers.Count}");
            }
        }

        [Server]
        public void RemovePlayer(GameObject player)
        {
            if (m_ServerPlayers.Remove(player))
            {
                DebugUtils.Log($"PlayerManager: '{player.name}' removed. Total players: {m_ServerPlayers.Count}");
            }
        }
    }
}