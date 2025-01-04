using UnityEngine;

namespace STARTING
{
    public class Managers : MonoBehaviour
    {
        private static Managers Instance { get; set; }

        //Manager
        public static GameManager Game { get; private set; }
        public static MapManager Map { get; private set; }
        public static UIManager UI { get; private set; }
        public static CustomNetworkManager Network { get; private set; }
        public static DBManager DB { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            Game = GetComponentInChildren<GameManager>();
            Map = GetComponentInChildren<MapManager>();
            UI = GetComponentInChildren<UIManager>();
            Network = GetComponentInChildren<CustomNetworkManager>();
            DB = GetComponentInChildren<DBManager>();
        }
    }
}