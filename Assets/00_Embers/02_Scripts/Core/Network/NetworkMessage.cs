using Mirror;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using static Michsky.UI.Reach.ChapterManager;

namespace STARTING
{
    // 로그인
    public struct LoginRequestMessage : NetworkMessage
    {
        public string username;
        public string password;
    }

    public struct LoginResponseMessage : NetworkMessage
    {
        public string username;
        public string email;
        public string createdDate;
        public LoginResult result;
    }

    // 회원가입
    public struct SignUpRequestMessage : NetworkMessage
    {
        public string username;
        public string password;
        public string email;
    }

    public struct SignUpResponseMessage : NetworkMessage
    {
        public SignUpResult result;
    }

    //프로필 정보 업데이트
    public struct ProfileUpdateRequestMessage : NetworkMessage
    {
        public string username;
        public string email;
        public string password;
    }

    public struct ProfileUpdateResponseMessage : NetworkMessage
    {
        public string email;
        public bool success;
    }

    
    //캐릭터 생성
    public struct CreateCharacterRequestMessage : NetworkMessage
    {
        public string username;
        public string characterName;
        public int faction;
        public int characterClass;
        public int gender;
        public int mapCode;
    }

    public struct CreateCharacterResponsetMessage : NetworkMessage
    {
        public CreateCharacterResult result;
    }

    //캐릭터 정보 로드

    public struct CharacterInfoLoadRequestMessage : NetworkMessage
    {
        public string username;
    }

    public struct CharacterInfoLoadResponseMessage : NetworkMessage
    {
        public List<ChapterItem> characterData;
    }

}