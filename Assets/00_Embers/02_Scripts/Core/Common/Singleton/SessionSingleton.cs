using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace NOLDA
{
    public class SessionSingleton : MonoBehaviour
    {

        [SerializeField] private readonly List<GameObject> m_ServerPlayers = new List<GameObject>();
        public IReadOnlyList<GameObject> ServerPlayers => m_ServerPlayers;


        [Server]
        public void AddPlayer(GameObject player)
        {
            if (!m_ServerPlayers.Contains(player))
            {
                m_ServerPlayers.Add(player);
                Debug.Log($"PlayerManager: '{player.name}' added. Total players: {m_ServerPlayers.Count}");
            }
        }

        [Server]
        public void RemovePlayer(GameObject player)
        {
            if (m_ServerPlayers.Remove(player))
            {
                Debug.Log($"PlayerManager: '{player.name}' removed. Total players: {m_ServerPlayers.Count}");
            }
        }
    }
}