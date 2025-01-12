using UnityEngine;
using System;

namespace STARTING
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "STARTING/Player Data", order = 1)]
    public class PlayerDataSO : ScriptableObject
    {
        public event Action<string, object> OnDataChanged;
        public event Action OnInventoryUpdated;

        public Item[] Items;
        
        public HxpTableSO hxpTable;
        public bool suppressEvents; //플래그가 true면 네트워크 전송을 멈춤


        #region #Inventory Data

        //인벤토리 처리 로직

        #endregion
        
        #region #Character Data
        // Account-related data
        [SerializeField] private string accountID;
        public string AccountID
        {
            get => accountID;
            set => accountID = value;
        }
        
        [SerializeField] private string email;
        public string Email
        {
            get => email;
            set => email = value;
        }
        
        [SerializeField] private string createdDate;
        public string CreatedDate
        {
            get => createdDate;
            set => createdDate = value;
        }

        // Character-related data
        [SerializeField] private string username;
        public string Username
        {
            get => username;
            set => username = value;
        }
        
        [SerializeField] private int level;
        public int Level
        {
            get => level;
            set
            {
                if (level != value)
                {
                    level = value;
                    if (false == suppressEvents)
                        OnDataChanged?.Invoke(nameof(Level), value);
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
                    if (false == suppressEvents)
                        OnDataChanged?.Invoke(nameof(MaxHp), value);
                    
                    Hp = Mathf.Clamp(Hp, 0, maxHp);
                }
            }
        }

        [SerializeField] private int hp;
        public int Hp
        {
            get => hp;
            set
            {
                int clampedValue = Mathf.Clamp(value, 0, MaxHp); // MaxHp를 초과하지 않도록 제한
                if (hp != clampedValue)
                {
                    hp = clampedValue;
                    if (false == suppressEvents)
                        OnDataChanged?.Invoke(nameof(Hp), hp);
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
                    if (false == suppressEvents)
                        OnDataChanged?.Invoke(nameof(MaxMp), value);
                    
                    Mp = Mathf.Clamp(Mp, 0, maxMp);
                }
            }
        }

        [SerializeField] private int mp;
        public int Mp
        {
            get => mp;
            set
            {
                int clampedValue = Mathf.Clamp(value, 0, MaxMp); // MaxHp를 초과하지 않도록 제한
                if (mp != clampedValue)
                {
                    mp = clampedValue;
                    if (false == suppressEvents)
                        OnDataChanged?.Invoke(nameof(Mp), mp);
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
                    if (false == suppressEvents)
                    {
                        OnDataChanged?.Invoke(nameof(Hxp), value);
                        CheckLevelUp();
                    }
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
                    if (false == suppressEvents)
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
                    if (false == suppressEvents)
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
                    if (false == suppressEvents)
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
                    if (false == suppressEvents)
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
                    if (false == suppressEvents)
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
                    if (false == suppressEvents)
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
                    if (false == suppressEvents)
                        OnDataChanged?.Invoke(nameof(MapCode), value);
                }
            }
        }
        
        [SerializeField] private int inventorySpace;
        public int InventorySpace
        {
            get => inventorySpace;
            set
            {
                if (inventorySpace != value)
                {
                    inventorySpace = value;
                    //InitializeInventory(value);
                    if (false == suppressEvents)
                        OnDataChanged?.Invoke(nameof(InventorySpace), value);
                }
            }
        }
        #endregion
        
        #region #각 프로퍼티 처리 Logic
        /// <summary>
        /// 경험치 상승시마다 레벨업하는지 체크
        /// </summary>
        private void CheckLevelUp()
        {
            while (Level < hxpTable.MaxLevel && Hxp >= hxpTable.GetExperienceForNextLevel(Level))
            {
                Hxp -= hxpTable.GetExperienceForNextLevel(Level);
                Level++;
                HandleLevelUp();
            }
        }

        private void HandleLevelUp()
        {
            //레벨업시 오르는 능력치 상승률
            MaxHp += 100;
            MaxMp += 100;
            Attack += 5;
            
            //레벨업하면 현재 HP, MP를 모두 채워줌
            Hp = MaxHp;
            Mp = MaxMp;
        }
        #endregion
    }
}
