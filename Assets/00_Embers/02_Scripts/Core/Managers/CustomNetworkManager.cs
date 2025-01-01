using Mirror;
using System.Collections.Generic;
using UnityEngine;
using static Michsky.UI.Reach.ChapterManager;

namespace STARTING
{
    public class CustomNetworkManager : NetworkManager
    {
        [Header("Server Info")]
        public string IP;
        public ushort PORT;

        public override void Start()
        {
#if UNITY_SERVER || UNITY_EDITOR
            if (!NetworkServer.active)
            {
                StartServer();
            }
#endif
            base.Start();
        }

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

            //DB Connect
            bool dbserver = Managers.DB.ConnectDB();
            Managers.Log.Log($"DB Connect? : {dbserver}");

            //Network Message Register
            NetworkServer.ReplaceHandler<LoginRequestMessage>(OnLoginRequest);
            NetworkServer.ReplaceHandler<SignUpRequestMessage>(OnRegisterRequest);
            NetworkServer.ReplaceHandler<ProfileUpdateRequestMessage>(OnProfileUpdateRequest);
            NetworkServer.ReplaceHandler<CreateCharacterRequestMessage>(OnCreateCharacterRequest);
            NetworkServer.ReplaceHandler<CharacterInfoLoadRequestMessage>(OnLoadCharacterInfoRequest);
            NetworkServer.ReplaceHandler<DeleteCharacterRequestMessage>(OnDeleteCharacterRequest);
            NetworkServer.ReplaceHandler<CharacterDataRequestMessage>(OnCharacterDataRequest);

            //Player Data Update
            NetworkServer.ReplaceHandler<UpdatePlayerDataMessage>(OnUpdatePlayerDataMessageReceived);


        }

        /// <summary>
        /// 클라이언트로부터 받은 로그인 요청 메시지
        /// 처리 결과에 따라 결과값 반환
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="msg"></param>
        private void OnLoginRequest(NetworkConnectionToClient conn, LoginRequestMessage msg)
        {
            LoginResponse result = Managers.DB.Login(msg.username, msg.password);

            LoginResponseMessage response = new LoginResponseMessage
            {
                username = msg.username,
                email = result.Email,
                createdDate = result.CreatedAt,
                result = result.Result,
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

        /// <summary>
        /// 클라이언트로부터 받은 프로필 정보 업데이트 요청 메시지
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="msg"></param>
        private void OnProfileUpdateRequest(NetworkConnectionToClient conn, ProfileUpdateRequestMessage msg)
        {
            bool result = Managers.DB.UpdateUserInfo(msg.username, msg.password, msg.email);

            ProfileUpdateResponseMessage response = new ProfileUpdateResponseMessage
            {
                email = msg.email,
                success = result
            };
            conn.Send(response);
        }

        /// <summary>
        /// 클라이언트 캐릭터 생성 요청 메시지
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="msg"></param>
        private void OnCreateCharacterRequest(NetworkConnectionToClient conn, CreateCharacterRequestMessage msg)
        {
            CreateCharacterResult result = Managers.DB.CreateCharacter(
                msg.username,
                msg.characterName,
                msg.faction,
                msg.characterClass,
                msg.gender,
                msg.mapCode);

            CreateCharacterResponsetMessage response = new CreateCharacterResponsetMessage
            {
                result = result
            };
            conn.Send(response);
        }
        
        /// <summary>
        /// 클라이언트가 로그인 했을 때 || 캐릭터를 새로 생성했을 때, 해당 계정의 캐릭터 정보 요청 메시지
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="msg"></param>
        private void OnLoadCharacterInfoRequest(NetworkConnectionToClient conn, CharacterInfoLoadRequestMessage msg)
        {
            List<ChapterItem> characterData = Managers.DB.GetCharactersByUsername(msg.username);

            CharacterInfoLoadResponseMessage response = new CharacterInfoLoadResponseMessage
            {
                characterData = characterData
            };
            conn.Send(response);
        }

        /// <summary>
        /// 클라이언트가 캐릭터 선택했을 때 해당 캐릭터에 대한 모든 정보 전달
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="msg"></param>
        public void OnCharacterDataRequest(NetworkConnectionToClient conn, CharacterDataRequestMessage msg)
        {
            PlayerDataSO playerData = Managers.DB.FetchPlayerDataFromDB(msg.username);

            CharacterDataResponseMessage message = new CharacterDataResponseMessage
            {
                Username = playerData.Username,
                Level = playerData.Level,
                Hp = playerData.Hp,
                Mp = playerData.Mp,
                Exp = playerData.Exp,
                Gold = playerData.Gold,
                MaxHp = playerData.MaxHp,
                MaxMp = playerData.MaxMp,
                Attack = playerData.Attack,
                Class = playerData.Class,
                Sp = playerData.Sp,
                Gender = playerData.Gender,
                Position = playerData.Position,
                MapCode = playerData.MapCode
            };

            conn.Send(message);
        }

        /// <summary>
        /// 캐릭터 삭제
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="msg"></param>
        public void OnDeleteCharacterRequest(NetworkConnectionToClient conn, DeleteCharacterRequestMessage msg)
        {
            bool result = Managers.DB.DeleteCharacter(msg.username);

            DeleteCharacterResponseMessage message = new DeleteCharacterResponseMessage
            {
                result = result
            };

            conn.Send(message);
        }


        /// <summary>
        /// 클라이언트 데이터 변경시 DB 데이터 변경 요청 메시지
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="message"></param>
        private void OnUpdatePlayerDataMessageReceived(NetworkConnectionToClient conn, UpdatePlayerDataMessage message)
        {
            Managers.DB.UpdateDatabase(message.username, message.fieldName, message.newValue);
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