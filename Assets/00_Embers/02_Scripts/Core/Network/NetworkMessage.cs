using Mirror;
using System.Collections.Generic;
using UnityEngine;
using static Michsky.UI.Reach.ChapterManager;

namespace STARTING
{
    // 로그인
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

    // 회원가입
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

    //프로필 정보 업데이트
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

    
    //캐릭터 생성
    public struct CreateCharacterRequestMessage : NetworkMessage
    {
        public string Username;
        public string CharacterName;
        public int Faction;
        public int CharacterClass;
        public int Gender;
        public int MapCode;
    }

    public struct CreateCharacterResponsetMessage : NetworkMessage
    {
        public CreateCharacterResult Result;
    }

    //캐릭터 정보 로드
    public struct CharacterInfoLoadRequestMessage : NetworkMessage
    {
        public string Username;
    }

    public struct CharacterInfoLoadResponseMessage : NetworkMessage
    {
        public List<ChapterItem> CharacterData;
    }

    //캐릭터 선택
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
        public string Class;
        public int Sp;
        public string Gender;
        public Vector3 Position;
        public string MapCode;
        public int InventorySpace;
        public List<InventoryItemMessage> InventoryItems;
    }

    public struct InventoryItemMessage
    {
        public int ItemId;
        public int Amount;
        public int Position;
    }
    
    //캐릭터 삭제
    public struct DeleteCharacterRequestMessage : NetworkMessage
    {
        public string Username;
    }

    public struct DeleteCharacterResponseMessage : NetworkMessage
    {
        public bool Result;
    }

    //PlayerDataSO update
    public struct UpdatePlayerDataMessage : NetworkMessage
    {
        public string Username;
        public string FieldName;
        public string NewValue;
    }
}