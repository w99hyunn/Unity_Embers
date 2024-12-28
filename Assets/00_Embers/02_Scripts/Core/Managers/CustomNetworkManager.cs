using Mirror;
using UnityEngine;

namespace STARTING
{
    public class CustomNetworkManager : NetworkManager
    {
        [Header("Server Info")]
        public string IP;
        public ushort PORT;

        public new void StartClient()
        {
            base.networkAddress = IP;
            if (Transport.active is PortTransport portTransport)
            {
                portTransport.Port = PORT;
            }

            base.StartClient();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            NetworkServer.RegisterHandler<LoginRequestMessage>(OnLoginRequest);
            NetworkServer.RegisterHandler<SignUpRequestMessage>(OnRegisterRequest);
            //NetworkServer.RegisterHandler<GameDataRequestMessage>(OnGameDataRequest);
            //NetworkServer.RegisterHandler<GameDataUpdateRequestMessage>(OnGameDataUpdateRequest);


            Debug.Log(Managers.DB.ConnectDB());


        }

        /// <summary>
        /// 클라이언트로부터 받은 로그인 요청 메시지
        /// 처리 결과에 따라 결과값 반환
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="msg"></param>
        private void OnLoginRequest(NetworkConnectionToClient conn, LoginRequestMessage msg)
        {
            LoginResult result = Managers.DB.Login(msg.username, msg.password);

            LoginResponseMessage response = new LoginResponseMessage
            {
                result = result
            };
            
            conn.Send(response);
        }

        /// <summary>
        /// 클라이언트로부터 받은 회원가입 요청 메시지
        /// 처리 결과에 따라 결과값 반환
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="msg"></param>
        private void OnRegisterRequest(NetworkConnectionToClient conn, SignUpRequestMessage msg)
        {
            SignUpResult result = Managers.DB.SignUp(msg.username, msg.password, msg.email);

            SignUpResponseMessage response = new SignUpResponseMessage
            {
                result = result
            };
            conn.Send(response);
        }


        //public override void OnServerDisconnect(NetworkConnectionToClient conn)
        //{
        //    if (SceneManager.GetActiveScene().name == SceneDataManager.GetSceneName("Main"))
        //    {
        //        return;
        //    }
        //    ChatManager.Instance?.CmdSendChatMessage("퇴장", $"{conn.identity.name}님이 퇴장하셨습니다.");
        //    base.OnServerDisconnect(conn);
        //}


        //private void OnGameDataRequest(NetworkConnection conn, GameDataRequestMessage msg)
        //{
        //    GameData gameData = DBManager.Instance.GetGameDataFromDB(msg.userId);

        //    GameDataResponseMessage response = new GameDataResponseMessage
        //    {
        //        gameData = gameData
        //    };

        //    conn.Send(response);
        //}

        //private void OnGameDataUpdateRequest(NetworkConnection conn, GameDataUpdateRequestMessage msg)
        //{
        //    DBManager.Instance.UpdateGameDataInDB(msg.userId, msg.gameData);
        //}

    }
}