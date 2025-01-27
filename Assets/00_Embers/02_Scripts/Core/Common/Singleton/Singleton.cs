using UnityEngine;

namespace NOLDA
{
    public class Singleton : MonoBehaviour
    {
        private static Singleton Instance { get; set; }

        //Manager
        public static GameSingleton Game { get; private set; }
        public static MapSingleton Map { get; private set; }
        public static UISingleton UI { get; private set; }
        public static NetworkSingleton Network { get; private set; }
        public static DBSingleton DB { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            Game = GetComponentInChildren<GameSingleton>();
            Map = GetComponentInChildren<MapSingleton>();
            UI = GetComponentInChildren<UISingleton>();
            Network = GetComponentInChildren<NetworkSingleton>();
            DB = GetComponentInChildren<DBSingleton>();
        }
    }
}