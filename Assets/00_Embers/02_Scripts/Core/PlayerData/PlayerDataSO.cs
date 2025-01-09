using UnityEngine;
using System;

namespace STARTING
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "STARTING/Player Data", order = 1)]
    public class PlayerDataSO : ScriptableObject
    {
        public event Action<string, object> OnDataChanged;

        // Account-related data
        public string AccountID;
        public string Email;
        public string CreatedDate;

        // Character-related data
        public string Username;
        
        [SerializeField] private int level;
        public int Level
        {
            get => level;
            set
            {
                if (level != value)
                {
                    level = value;
                    OnDataChanged?.Invoke(nameof(Level), value);
                }
            }
        }

        [SerializeField] private int hp;
        public int Hp
        {
            get => hp;
            set
            {
                if (hp != value)
                {
                    hp = value;
                    OnDataChanged?.Invoke(nameof(Hp), value);
                }
            }
        }

        [SerializeField] private int maxHp;
        public int MaxHp
        {
            get => maxHp;
            set
            {
                if (maxHp != value)
                {
                    maxHp = value;
                    OnDataChanged?.Invoke(nameof(MaxHp), value);
                }
            }
        }

        [SerializeField] private int mp;
        public int Mp
        {
            get => mp;
            set
            {
                if (mp != value)
                {
                    mp = value;
                    OnDataChanged?.Invoke(nameof(Mp), value);
                }
            }
        }

        [SerializeField] private int maxMp;
        public int MaxMp
        {
            get => maxMp;
            set
            {
                if (maxMp != value)
                {
                    maxMp = value;
                    OnDataChanged?.Invoke(nameof(MaxMp), value);
                }
            }
        }

        [SerializeField] private int hxp;
        public int Hxp
        {
            get => hxp;
            set
            {
                if (hxp != value)
                {
                    hxp = value;
                    OnDataChanged?.Invoke(nameof(Hxp), value);
                }
            }
        }

        [SerializeField] private int gold;
        public int Gold
        {
            get => gold;
            set
            {
                if (gold != value)
                {
                    gold = value;
                    OnDataChanged?.Invoke(nameof(Gold), value);
                }
            }
        }

        [SerializeField] private int attack;
        public int Attack
        {
            get => attack;
            set
            {
                if (attack != value)
                {
                    attack = value;
                    OnDataChanged?.Invoke(nameof(Attack), value);
                }
            }
        }

        [SerializeField] private string classType;
        public string Class
        {
            get => classType;
            set
            {
                if (classType != value)
                {
                    classType = value;
                    OnDataChanged?.Invoke(nameof(Class), value);
                }
            }
        }

        [SerializeField] private int sp;
        public int Sp
        {
            get => sp;
            set
            {
                if (sp != value)
                {
                    sp = value;
                    OnDataChanged?.Invoke(nameof(Sp), value);
                }
            }
        }

        [SerializeField] private string gender;
        public string Gender
        {
            get => gender;
            set
            {
                if (gender != value)
                {
                    gender = value;
                    OnDataChanged?.Invoke(nameof(Gender), value);
                }
            }
        }

        // World-related data
        [SerializeField] private Vector3 position;
        public Vector3 Position
        {
            get => position;
            set
            {
                if (position != value)
                {
                    position = value;
                    string positionJson = JsonUtility.ToJson(position);
                    OnDataChanged?.Invoke(nameof(Position), positionJson);
                }
            }
        }

        [SerializeField] private string mapCode;
        public string MapCode
        {
            get => mapCode;
            set
            {
                if (mapCode != value)
                {
                    mapCode = value;
                    OnDataChanged?.Invoke(nameof(MapCode), value);
                }
            }
        }
    }
}
