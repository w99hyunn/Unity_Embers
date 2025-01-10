using Mirror;
using System.Collections.Generic;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Michsky.UI.Reach.ChapterManager;

namespace STARTING
{
    public class CustomNetworkManager : NetworkManager
    {
        [Header("Server Info")]
        public string IP;
        public ushort PORT;
        public bool _SERVER_AUTO_RUN = false;

        public override void Start()
        {
#if UNITY_EDITOR
            try
            {
                if (CurrentPlayer.ReadOnlyTags()[0] == "Server")
                {
                    SceneManager.UnloadSceneAsync("Title");
                    StartServer();
                }
            }
            catch
            {
                DebugUtils.Log("Client 동작 실행");
            }
            
            if (true == _SERVER_AUTO_RUN)
            {
                if (!NetworkServer.active)
                {
                    StartServer();
                }
            }
#endif
#if UNITY_SERVER
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
            bool dbServer = Managers.DB.ConnectDB();
            DebugUtils.Log($"DB Connect? : {dbServer}");

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

        public override void OnStopServer()
        {
            Managers.DB.CloseDBServer();
            base.OnStopServer();
        }

        public override void OnClientConnect()
        {
            //기존의 NetworkManager의 Ready()와 AddPlayer()를 따로 해주기 때문에 공란
        }

        /// <summary>
        /// 클라이언트로부터 받은 로그인 요청 메시지
        /// 처리 결과에 따라 결과값 반환
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="msg"></param>
        private void OnLoginRequest(NetworkConnectionToClient conn, LoginRequestMessage msg)
        {
            LoginResponse result = Managers.DB.Login(msg.Username, msg.Password);

            LoginResponseMessage response = new LoginResponseMessage
            {
                Username = msg.Username,
                Email = result.Email,
                CreatedDate = result.CreatedAt,
                Result = result.Result,
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
            SignUpResult result = Managers.DB.SignUp(msg.Username, msg.Password, msg.Email);

            SignUpResponseMessage response = new SignUpResponseMessage
            {
                Result = result
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
            bool result = Managers.DB.UpdateUserInfo(msg.Username, msg.Password, msg.Email);

            ProfileUpdateResponseMessage response = new ProfileUpdateResponseMessage
            {
                Email = msg.Email,
                Success = result
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
                msg.Username,
                msg.CharacterName,
                msg.Faction,
                msg.CharacterClass,
                msg.Gender,
                msg.MapCode);

            CreateCharacterResponsetMessage response = new CreateCharacterResponsetMessage
            {
                Result = result
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
            List<ChapterItem> characterData = Managers.DB.GetCharactersByUsername(msg.Username);

            CharacterInfoLoadResponseMessage response = new CharacterInfoLoadResponseMessage
            {
                CharacterData = characterData
            };
            conn.Send(response);
        }

        /// <summary>
        /// 클라이언트가 캐릭터 선택했을 때 해당 캐릭터에 대한 모든 정보 전달
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="msg"></param>
        private void OnCharacterDataRequest(NetworkConnectionToClient conn, CharacterDataRequestMessage msg)
        {
            PlayerDataSO playerData = Managers.DB.FetchPlayerDataFromDB(msg.Username);

            CharacterDataResponseMessage message = new CharacterDataResponseMessage
            {
                Username = playerData.Username,
                Level = playerData.Level,
                Hp = playerData.Hp,
                Mp = playerData.Mp,
                Hxp = playerData.Hxp,
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
        private void OnDeleteCharacterRequest(NetworkConnectionToClient conn, DeleteCharacterRequestMessage msg)
        {
            bool result = Managers.DB.DeleteCharacter(msg.Username);

            DeleteCharacterResponseMessage message = new DeleteCharacterResponseMessage
            {
                Result = result
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
            Managers.DB.UpdateDatabase(message.Username, message.FieldName, message.NewValue);
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