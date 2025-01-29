using UnityEngine;

namespace NOLDA
{
    public class Director : MonoBehaviour
    {
        private static Director Instance { get; set; }

        //Manager
        public static GameDirector Game { get; private set; }
        public static MapDirector Map { get; private set; }
        public static UIDirector UI { get; private set; }
        public static NetworkDirector Network { get; private set; }
        public static DBDirector DB { get; private set; }
        public static SkillDirector Skill { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            Game = GetComponentInChildren<GameDirector>();
            Map = GetComponentInChildren<MapDirector>();
            UI = GetComponentInChildren<UIDirector>();
            Network = GetComponentInChildren<NetworkDirector>();
            DB = GetComponentInChildren<DBDirector>();
            Skill = GetComponentInChildren<SkillDirector>();
        }
    }
}