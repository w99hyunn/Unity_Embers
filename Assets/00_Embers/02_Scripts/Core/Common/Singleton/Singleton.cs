using UnityEngine;

namespace NOLDA
{
    public class Singleton : MonoBehaviour
    {
        private static Singleton Instance { get; set; }

        //Managers
        public static GameSingleton Game { get; private set; }
        public static UISingleton UI { get; private set; }
        public static NetworkSingleton Network { get; private set; }
        public static DBSingleton DB { get; private set; }
        public static SkillSingleton Skill { get; private set; }

        public GameSingleton gameSingleton;
        public UISingleton uISingleton;
        public NetworkSingleton networkSingleton;
        public DBSingleton dbSingleton;
        public SkillSingleton skillSingleton;


        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            Game = gameSingleton;
            UI = uISingleton;
            Network = networkSingleton;
            DB = dbSingleton;
            Skill = skillSingleton;
        }
    }
}