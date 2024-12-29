using Mirror;
using System;

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

    //


    //// 계정 정보 클라이언트가 요청 후 Edit Profile Popup에 띄우기 위함
    //public struct ProfileInfoRequestMessage : NetworkMessage
    //{
    //    public string username;
    //}

    //public struct ProfileInfoResponseMessage : NetworkMessage
    //{
    //    public string username;
    //    public string email;
    //}
}