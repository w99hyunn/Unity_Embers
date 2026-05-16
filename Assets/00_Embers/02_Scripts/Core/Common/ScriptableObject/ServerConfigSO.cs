using UnityEngine;

namespace Embers
{
    [CreateAssetMenu(fileName = "ServerConfig", menuName = "NOLDA/ServerConfig", order = 1)]
    public class ServerConfigSO : ScriptableObject
    {
        [Header("Server Info")]
        public string serverIp = "localhost";
        public ushort serverPort = 8585;
        public bool serverAutoRun = false;

        [Header("DB Server Info")]
        public string dbServerIP = "localhost";
        public string dbHost = "root";
        public string dbPw = "root";
        public string dbPort = "3306";
    }
}

