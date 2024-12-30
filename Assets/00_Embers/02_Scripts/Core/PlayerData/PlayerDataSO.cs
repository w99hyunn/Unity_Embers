using UnityEngine;

namespace STARTING
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "STARTING/Player Data", order = 1)]
    public class PlayerDataSO : ScriptableObject
    {
        // Account-related data
        public string AccountID;
        public string Email;
        public string CreatedDate;

        // Character-related data
        public string Username;
        public int Level;
        public int Hp;
        public int Mp;
        public int Exp;
        public int Gold;
        public int MaxHp;
        public int MaxMp;
        public int Attack;
        public string Class;
        public int Sp;
        public string Gender;

        // World-related data
        public Vector3 Position;
        public string MapCode;

        /// <summary>
        /// Initializes character-related data.
        /// </summary>
        /// <param name="username">Character's name</param>
        /// <param name="level">Character's level</param>
        /// <param name="hp">Current HP</param>
        /// <param name="mp">Current MP</param>
        /// <param name="exp">Current experience</param>
        /// <param name="gold">Current gold</param>
        /// <param name="maxHp">Maximum HP</param>
        /// <param name="maxMp">Maximum MP</param>
        /// <param name="attack">Attack power</param>
        /// <param name="classType">Class name</param>
        /// <param name="sp">Skill points</param>
        /// <param name="gender">Gender</param>
        /// <param name="position">World position</param>
        /// <param name="mapCode">Current map code</param>
        public void Initialize(
            string username,
            int level,
            int hp,
            int mp,
            int exp,
            int gold,
            int maxHp,
            int maxMp,
            int attack,
            string classType,
            int sp,
            string gender,
            Vector3 position,
            string mapCode)
        {
            Username = username;
            Level = level;
            Hp = hp;
            Mp = mp;
            Exp = exp;
            Gold = gold;
            MaxHp = maxHp;
            MaxMp = maxMp;
            Attack = attack;
            Class = classType;
            Sp = sp;
            Gender = gender;
            Position = position;
            MapCode = mapCode;
        }
    }
}