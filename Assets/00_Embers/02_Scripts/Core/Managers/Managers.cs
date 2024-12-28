using UnityEngine;

namespace STARTING
{
    public class Managers : MonoBehaviour
    {
        public static Managers Instance { get; private set; }

        //Manager
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

            Network = GetComponentInChildren<CustomNetworkManager>();
            DB = GetComponentInChildren<DBManager>();
            Log = GetComponentInChildren<LogManager>();
        }
    }
}