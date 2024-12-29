using UnityEngine;

namespace STARTING
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "STARTING/Player Data", order = 1)]
    public class PlayerDataSO : ScriptableObject
    {
        public string AccountID;
        public string Username;
        public int Level;
        public int Hp;
        public int Mp;
        public int Exp;
        public int Gold;
        public Vector3 Position;
        public string MapCode;

        public void Initialize(string username, int level, int hp, int mp, int exp, int gold, Vector3 position, string mapCode)
        {
            Username = username;
            Level = level;
            Hp = hp;
            Mp = mp;
            Exp = exp;
            Gold = gold;
            Position = position;
            MapCode = mapCode;
        }
    }
}