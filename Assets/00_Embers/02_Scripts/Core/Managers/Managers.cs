using UnityEngine;

namespace STARTING
{
    public class Managers : MonoBehaviour
    {
        private static Managers Instance { get; set; }

        //Manager
        public static GameManager Game { get; private set; }
        public static CustomNetworkManager Network { get; private set; }
        public static DBManager DB { get; private set; }
        public static LogManager Log { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            Game = GetComponentInChildren<GameManager>();
            Network = GetComponentInChildren<CustomNetworkManager>();
            DB = GetComponentInChildren<DBManager>();
            Log = GetComponentInChildren<LogManager>();
        }
    }
}