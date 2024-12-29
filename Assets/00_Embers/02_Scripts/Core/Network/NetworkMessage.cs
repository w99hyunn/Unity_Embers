using Mirror;

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
    //

    // 게임 데이터 로드
    public struct GameDataRequestMessage : NetworkMessage
    {
        public int userId;
    }

}