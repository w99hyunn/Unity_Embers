using Mirror;
using System.Collections.Generic;
using Unity.Multiplayer.Playmode;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Michsky.UI.Reach.ChapterManager;

namespace NOLDA
{
    public class NetworkDirector : NetworkManager
    {
        [SerializeField] private ChatServer chatServer;
        public ChatServer ChatServer => chatServer;
        
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
            
            if (true == Director.Game.ServerAutoRun)
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
            base.networkAddress = Director.Game.ServerIP;
            if (Transport.active is PortTransport portTransport)
            {
                portTransport.Port = Director.Game.ServerPort;
            }

            base.StartClient();
        }
        
        public override void OnStartServer()
        {
            base.OnStartServer();

            //DB Connect
            bool dbServer = Director.DB.ConnectDB();
            DebugUtils.Log($"DB 연결 유무 : {dbServer}");

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
            
            //Player Inventory Update
            NetworkServer.ReplaceHandler<UpdateInventoryMessage>(OnUpdateInventoryMessageReceived);

            //Player Skill Update
            NetworkServer.RegisterHandler<UpdateSkillMessage>(OnUpdateSkillMessageReceived);
        }

        public override void OnStopServer()
        {
            Director.DB.CloseDBServer();
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
            LoginResponse result = Director.DB.Login(msg.Username, msg.Password);

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
            SignUpResult result = Director.DB.SignUp(msg.Username, msg.Password, msg.Email);

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
            bool result = Director.DB.UpdateUserInfo(msg.Username, msg.Password, msg.Email);

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
            CreateCharacterResult result = Director.DB.CreateCharacter(
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
            List<ChapterItem> characterData = Director.DB.GetCharactersByUsername(msg.Username);

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
            PlayerDataSO playerData = Director.DB.FetchPlayerDataFromDB(msg.Username);

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
                Faction = playerData.Faction,
                Sp = playerData.Sp,
                Gender = playerData.Gender,
                Position = playerData.Position,
                MapCode = playerData.MapCode,
                InventorySpace = playerData.InventorySpace,
                InventoryItems = new List<InventoryItemMessage>(),
                Skills = new List<SkillEntry>()
            };

            foreach (var skill in playerData.Skills)
            {
                message.Skills.Add(new SkillEntry(skill.Key, skill.Value));
            }

            for (int i = 0; i < playerData.Items.Length; i++)
            {
                Item item = playerData.Items[i];
                if (item != null)
                {
                    InventoryItemMessage itemMessage = new InventoryItemMessage
                    {
                        ItemId = item.Data.ID, // 아이템 ID
                        Amount = (item is CountableItem countableItem) ? countableItem.Amount : 1, // CountableItem일 경우 수량, 아니면 기본 1
                        Position = i // 배열에서의 인덱스(슬롯 위치)
                    };

                    message.InventoryItems.Add(itemMessage);
                }
            }

            conn.Send(message);
        }

        /// <summary>
        /// 캐릭터 삭제
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="msg"></param>
        private void OnDeleteCharacterRequest(NetworkConnectionToClient conn, DeleteCharacterRequestMessage msg)
        {
            bool result = Director.DB.DeleteCharacter(msg.Username);

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
            Director.DB.UpdateDatabase(message.CharacterName, message.FieldName, message.NewValue);
        }

        /// <summary>
        /// 클라이언트의 인벤토리에 변화가 생겼을 때 DB 업데이트 요청 메시지
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="msg"></param>
        private void OnUpdateInventoryMessageReceived(NetworkConnectionToClient conn, UpdateInventoryMessage msg)
        {
            // 슬롯 업데이트 요청 처리
            Director.DB.HandleSlotUpdateInDB(msg.CharacterName, msg.Index, msg.ItemId, msg.Amount);
        }

        /// <summary>
        /// 클라이언트의 스킬 레벨 업데이트 요청 메시지
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="msg"></param>
        private void OnUpdateSkillMessageReceived(NetworkConnectionToClient conn, UpdateSkillMessage msg)
        {
            Director.DB.UpdateCharacterSkillsInDB(msg.CharacterName, msg.SkillID, msg.Level);
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
    }
}