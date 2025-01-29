using Mirror;
using System.Collections.Generic;
using UnityEngine;
using static Michsky.UI.Reach.ChapterManager;

namespace NOLDA
{
    #region # 로그인
    public struct LoginRequestMessage : NetworkMessage
    {
        public string Username;
        public string Password;
    }

    public struct LoginResponseMessage : NetworkMessage
    {
        public string Username;
        public string Email;
        public string CreatedDate;
        public LoginResult Result;
    }
    #endregion

    #region # 회원가입
    public struct SignUpRequestMessage : NetworkMessage
    {
        public string Username;
        public string Password;
        public string Email;
    }

    public struct SignUpResponseMessage : NetworkMessage
    {
        public SignUpResult Result;
    }
    #endregion

    #region # 프로필 정보 업데이트
    public struct ProfileUpdateRequestMessage : NetworkMessage
    {
        public string Username;
        public string Email;
        public string Password;
    }

    public struct ProfileUpdateResponseMessage : NetworkMessage
    {
        public string Email;
        public bool Success;
    }
    #endregion

    #region # 캐릭터 생성
    public struct CreateCharacterRequestMessage : NetworkMessage
    {
        public string Username;
        public string CharacterName;
        public Faction Faction;
        public Class CharacterClass;
        public Gender Gender;
        public int MapCode;
    }

    public struct CreateCharacterResponsetMessage : NetworkMessage
    {
        public CreateCharacterResult Result;
    }
    #endregion
    
    #region # 캐릭터 리스트 로드
    public struct CharacterInfoLoadRequestMessage : NetworkMessage
    {
        public string Username;
    }

    public struct CharacterInfoLoadResponseMessage : NetworkMessage
    {
        public List<ChapterItem> CharacterData;
    }
    #endregion
    
    #region # 캐릭터 선택시 정보 로드
    public struct CharacterDataRequestMessage : NetworkMessage
    {
        public string Username;
    }

    public struct CharacterDataResponseMessage : NetworkMessage
    {
        public string Username;
        public int Level;
        public int Hp;
        public int Mp;
        public int Hxp;
        public int Gold;
        public int MaxHp;
        public int MaxMp;
        public int Attack;
        public Class Class;
        public Faction Faction;
        public int Sp;
        public Gender Gender;
        public Vector3 Position;
        public string MapCode;
        public int InventorySpace;
        public List<InventoryItemMessage> InventoryItems;
        public List<SkillEntry> Skills;
    }
    
    [System.Serializable]
    public struct SkillEntry
    {
        public int SkillID;
        public int Level;

        public SkillEntry(int skillID, int level)
        {
            SkillID = skillID;
            Level = level;
        }
    }

    public struct InventoryItemMessage
    {
        public int ItemId;
        public int Amount;
        public int Position;
    }
    #endregion
    
    #region # 캐릭터 삭제
    public struct DeleteCharacterRequestMessage : NetworkMessage
    {
        public string Username;
    }

    public struct DeleteCharacterResponseMessage : NetworkMessage
    {
        public bool Result;
    }
    #endregion
    
    #region # 캐릭터 데이터 업데이트
    //PlayerDataSO update
    public struct UpdatePlayerDataMessage : NetworkMessage
    {
        public string CharacterName;
        public string FieldName;
        public string NewValue;
    }
    
    //Inventory Data update
    public struct UpdateInventoryMessage : NetworkMessage
    {
        public string CharacterName;
        public int Index;
        public int ItemId;
        public int Amount;
    }

    //Skill Data update
    public struct UpdateSkillMessage : NetworkMessage
    {
        public string CharacterName;
        public int SkillID;
        public int Level;
    }
    #endregion
}