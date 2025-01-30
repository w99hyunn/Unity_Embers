using UnityEngine;
using System;
using System.Collections.Generic;
namespace NOLDA
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "NOLDA/PlayerData", order = 1)]
    /// <summary>
    /// Username = 캐릭터 이름
    /// Level = 캐릭터 레벨
    /// TotalAttack = 최종 공격력 (Attack + additionalAttack)
    /// TotalArmor = 최종 방어력 (Armor + additionalArmor)
    /// TotalMaxHp = 최종 MaxHP (MaxHp + additionalMaxHp)
    /// TotalMaxMp = 최종 MaxMP (MaxMp + additionalMaxMp)
    /// Gold = 골드
    /// Faction = 세력
    /// Class = 직업
    /// Sp = 스킬 포인트
    /// Gender = 성별
    /// Position = 위치
    /// MapCode = 맵 코드(현재 미사용 - 추후 인스턴스 던전에서 필요할듯)
    /// InventorySpace = 인벤토리 슬롯 수
    /// 
    /// 공격력, 방어력, MaxHP, MaxMP 불러올 때 주의할 것. Total 값으로 불러와야함.
    /// additional은 스킬 또는 아이템에서 추가 능력치 적용되는 수치
    /// </summary>
    public class PlayerDataSO : ScriptableObject
    {
        public event Action<string, object> OnDataChanged;
        public event Action OnPassiveSkillsApplied;

        public HxpTableSO hxpTable;
        
        public bool suppressEvents; //플래그가 true면 네트워크 전송을 멈춤

        #region # 추가 능력치(스킬, 아이템 등)가 적용된 최종 능력치
        //추가 능력치치
        [SerializeField] private int additionalAttack;
        [SerializeField] private int additionalArmor;
        [SerializeField] private int additionalMaxHp;
        [SerializeField] private int additionalMaxMp;

        //외부에서 참조해야할 최종 능력치
        public int TotalAttack => Attack + additionalAttack;
        public int TotalArmor => Armor + additionalArmor;
        public int TotalMaxHp => MaxHp + additionalMaxHp;
        public int TotalMaxMp => MaxMp + additionalMaxMp;

        //추가 능력치 적용 메소드
        public void ApplyAdditionalStats(int maxHp, int maxMp, int defense, int attack)
        {
            additionalMaxHp += maxHp;
            additionalMaxMp += maxMp;
            additionalArmor += defense;
            additionalAttack += attack;
            OnPassiveSkillsApplied?.Invoke();
        }
        public void ResetAdditionalStats()
        {
            additionalMaxHp = 0;
            additionalMaxMp = 0;
            additionalArmor = 0;
            additionalAttack = 0;
        }
        #endregion
        
        #region # Skill Data
        [SerializeField] private Dictionary<int, int> skills = new Dictionary<int, int>();

        public Dictionary<int, int> Skills
        {
            get => skills;
            set => skills = value;
        }

        public bool LearnSkill(int skillID)
        {
            if (skills.ContainsKey(skillID)) return false;

            SkillData skill = Director.Skill.GetSkillData(skillID);
            if (skill == null) return false;

            SkillLevelData firstLevelData = skill.levelData[0];
            if (Level < firstLevelData.requiredLevel) return false;

            if (Sp < 1) return false;

            //스킬 배우기 가능할시
            skills[skillID] = 1;
            Sp--;
            Director.Game.SendSkillUpdateToServer(Username, skillID, 1);
            if (skill.skillType == SkillType.PASSIVE)
            {
                ApplyPassiveSkills();
            }
            return true;
        }

        public bool LevelUpSkill(int skillID)
        {
            if (!skills.ContainsKey(skillID)) return false;

            SkillData skill = Director.Skill.GetSkillData(skillID);
            if (skill == null) return false;

            int currentLevel = skills[skillID];
            if (currentLevel >= skill.levelData.Count) return false;

            SkillLevelData nextLevelData = skill.levelData[currentLevel];
            if (Level < nextLevelData.requiredLevel) return false;

            if (Sp < 1) return false;

            //스킬 업데이트 가능할시
            skills[skillID]++;
            Sp--;
            Director.Game.SendSkillUpdateToServer(Username, skillID, skills[skillID]);
            if (skill.skillType == SkillType.PASSIVE)
            {
                ApplyPassiveSkills();
            }
            return true;
        }

        public void ApplyPassiveSkills()
        {
            Director.Game.playerData.ResetAdditionalStats();
            foreach (var skillEntry in Director.Game.playerData.Skills)
            {
                SkillData skillData = Director.Skill.GetSkillData(skillEntry.Key);
                if (skillData == null || skillData.skillType != SkillType.PASSIVE) continue;

                SkillLevelData levelData = skillData.GetSkillLevelData(skillEntry.Value);
                if (levelData == null) continue;

                // 추가 능력치 적용
                Director.Game.playerData.ApplyAdditionalStats(
                    levelData.maxHpIncrease,
                    levelData.maxMpIncrease,
                    levelData.armorIncrease,
                    levelData.attackIncrease
                );
            }
        }

        #endregion

        #region # Inventory Data
        [SerializeField] private Item[] items;

        public Item[] Items
        {
            get => items;
            set => items = value;
        }
        #endregion
        
        #region # Character Data
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

        [SerializeField] private int armor;
        public int Armor
        {
            get => armor;
            set
            {
                if (armor != value)
                {
                    armor = value;
                    if (false == suppressEvents)
                        OnDataChanged?.Invoke(nameof(Armor), value);
                }
            }
        }

        [SerializeField] private Faction factionType;
        public Faction Faction
        {
            get => factionType;
            set
            {
                if (factionType != value)
                {
                    factionType = value;
                    if (false == suppressEvents)
                        OnDataChanged?.Invoke(nameof(Faction), value);
                }
            }
        }


        [SerializeField] private Class classType;
        public Class Class
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

        [SerializeField] private Gender gender;
        public Gender Gender
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
        
        #region # 각 프로퍼티 처리 Logic
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
            //레벨업시 오르는 능력치 상승률(GameSingleton에서 정의)
            MaxHp += Director.Game.LevelUpMaxHp;
            MaxMp += Director.Game.LevelUpMaxMp;
            Attack += Director.Game.LevelUpAttack;
            Armor += Director.Game.LevelUpArmor;
            Sp += Director.Game.LevelUpSp;

            //레벨업하면 현재 HP, MP를 모두 채워줌
            Hp = TotalMaxHp;
            Mp = TotalMaxMp;
        }
        #endregion
    }
}
