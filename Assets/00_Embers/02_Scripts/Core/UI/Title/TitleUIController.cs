using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace STARTING
{
    public class TitleUIController : MonoBehaviour
    {
        private TitleUIView _view;
        private TitleUIModel _model;

        private const int SERVER_CONNECT_MAX_RETRY = 10;
        private bool _isCheckingConnection;

        private void Start()
        {
            TryGetComponent<TitleUIView>(out _view);
            TryGetComponent<TitleUIModel>(out _model);
        }

        #region #General
        public async void ServerConnect()
        {
            await WaitForNetworkInitialization();
            await CheckConnectionAsync();
        }

        private async Awaitable WaitForNetworkInitialization()
        {
            while (Managers.Network == null)
            {
                await Awaitable.NextFrameAsync();
            }
        }

        /// <summary>
        /// 접속시 서버 연결
        /// </summary>
        /// <returns></returns>
        private async Awaitable CheckConnectionAsync()
        {
            var attemptCount = 0;
            var connected = false;

            while (attemptCount < SERVER_CONNECT_MAX_RETRY && !connected)
            {
                Managers.Network.StartClient();
                attemptCount++;

                _view.ConnectingMessageUpdate($"Try {attemptCount} / {SERVER_CONNECT_MAX_RETRY}");

                //연결됐는지 확인
                while (NetworkClient.active && !NetworkClient.isConnected)
                {
                    await Awaitable.NextFrameAsync();
                }

                // 연결 성공
                if (NetworkClient.isConnected)
                {
                    _view.ConnectingSuccess();
                    _ = CheckServerConnectionLoop();
                    connected = true;
                }
                else
                {
                    await Awaitable.WaitForSecondsAsync(2f);
                }
            }

            // 10번 재시도 후에도 연결되지 않으면 실패
            if (!connected)
            {
                _view.ConnectingFail();
                Managers.UI.OpenAlert("CONNECTING FAIL",
                "The server cannot be connected. If you continue to fail to connect, please contact us on the website.");
            }
        }


        private async Awaitable CheckServerConnectionLoop()
        {
            DebugUtils.Log("서버 연결상태 확인 시작");
            _isCheckingConnection = true;

            while (_isCheckingConnection)
            {
                await Awaitable.WaitForSecondsAsync(15f); // 15초 대기

                // 서버 연결 상태 확인
                if (!NetworkClient.isConnected)
                {
                    HandleServerDisconnection();
                    break; // 연결이 끊기면 루프 종료
                }
            }
        }

        /// <summary>
        /// 서버 연결 끊김 처리
        /// </summary>
        private void HandleServerDisconnection()
        {
            Managers.UI.OpenAlert("CONNECTING LOST",
                "The connection to the server is lost, and the game is terminated.", 1);
            _isCheckingConnection = false;
        }

        /// <summary>
        /// 회원가입
        /// </summary>
        public void SignUp()
        {
            //아이디 길이 확인
            if (_view.SignUpID.Length < 5)
            {
                Managers.UI.OpenAlert("FAIL", "ID must be at least 5 characters long.");
                return;
            }
            
            //비밀번호 길이 확인
            if (_view.SignUpPw.Length < 5)
            {
                Managers.UI.OpenAlert("FAIL", "Password must be at least 5 characters long.");
                return;
            }
            
            //비밀번호 제대로 입력했는지 체크
            if (_view.SignUpPw != _view.SignUpPwConfirm)
            {
                Managers.UI.OpenAlert("FAIL", "Invalid password input, please enter the same value.");
                return;
            }
            
            SignUpRequest(_view.SignUpID, _view.SignUpPw, _view.SignUpEmail);
        }


        private void SignUpRequest(string username, string password, string email)
        {
            SignUpRequestMessage signUpRequestMessage = new SignUpRequestMessage
            {
                Username = username,
                Password = password,
                Email = email
            };

            NetworkClient.ReplaceHandler<SignUpResponseMessage>(OnSignUpResultReceived);
            NetworkClient.Send(signUpRequestMessage);
        }

        private void OnSignUpResultReceived(SignUpResponseMessage msg)
        {
            string title;
            string message;

            switch (msg.Result)
            {
                case SignUpResult.SUCCESS:
                    title = "SUCCESS";
                    message = "Sign Up was successful.";
                    _view.SignUpSuccess();
                    break;
                case SignUpResult.DUPLICATE:
                    title = "DUPLICATE";
                    message = "This is a duplicate ID. Please use a different ID.";
                    break;
                case SignUpResult.ERROR:
                default:
                    title = "ERROR";
                    message = "An error has occurred. Please try again.";
                    break;
            }

            Managers.UI.OpenAlert(title, message);
        }

        /// <summary>
        /// 로그인
        /// </summary>
        public void Login()
        {
            LoginRequest(_view.LoginID, _view.LoginPw);
        }

        private void LoginRequest(string username, string password)
        {
            LoginRequestMessage loginRequestMessage = new LoginRequestMessage
            {
                Username = username,
                Password = password
            };

            NetworkClient.ReplaceHandler<LoginResponseMessage>(OnLoginResultReceived);
            NetworkClient.Send(loginRequestMessage);
        }

        private void OnLoginResultReceived(LoginResponseMessage msg)
        {
            string title;
            string message;

            switch (msg.Result)
            {
                case LoginResult.SUCCESS:
                    Managers.Game.LoginSuccess(msg.Username, msg.Email, msg.CreatedDate);
                    _view.menuManager.DisableSplashScreen();
                    _view.TopPanelProfileUpdate();
                    LoadCharacterInfo();
                    return;
                case LoginResult.IDWRONG:
                    title = "ID WRONG";
                    message = "This ID does not exist.";
                    break;
                case LoginResult.PWWRONG:
                    title = "PASSWORD WRONG";
                    message = "Password does not match.";
                    break;
                case LoginResult.ERROR:
                default:
                    title = "ERROR";
                    message = "An error has occurred. Please try again.";
                    break;
            }

            Managers.UI.OpenAlert(title, message);
        }
        
        public void ReturnBackTitle()
        {
            _view.TopPanelProfileUpdate();
            LoadCharacterInfo();
            _view.OpenPanel("GameStart");
        }
        
        public void EditProfilePopupOpen()
        {
            _view.EditProfilePopupInit();
        }

        public void EditProfileConfirm()
        {
            if (_view.EditProfilePw != _view.EditProfilePwConfirm)
            {
                //뭐라도 값이 입력됐는데 두개 필드가 다르면 안내
                Managers.UI.OpenAlert("FAIL", "Invalid password input, please enter the same value.");
                return;
            }

            EditProfileUpdate(_view.EditProfilePw, _view.EditProfileEmail);

        }

        private void EditProfileUpdate(string password, string email)
        {
            EditProfileRequest(password, email);
        }

        private void EditProfileRequest(string password, string email)
        {
            ProfileUpdateRequestMessage profileUpdateRequestMessage = new ProfileUpdateRequestMessage
            {
                Username = Managers.Game.playerData.AccountID,
                Email = email,
                Password = password
            };

            NetworkClient.ReplaceHandler<ProfileUpdateResponseMessage>(OnProfileUpdateResultReceived);
            NetworkClient.Send(profileUpdateRequestMessage);
        }

        private void OnProfileUpdateResultReceived(ProfileUpdateResponseMessage msg)
        {
            if (true == msg.Success)
            {
                Managers.Game.UserInfoUpdate(msg.Email);
                Managers.UI.OpenAlert("SUCCESS", "User information update successful.");
                _view.EditProfileUpdateSuccess();
            }
            else
            {
                Managers.UI.OpenAlert("FAIL", "Failed to update user information.");
            }
        }
        #endregion

        #region #Character
        public void CreateCharacter()
        {
            CreateCharacterRequest(
                Managers.Game.playerData.AccountID,
                _view.Name,
                _view.Faction,
                _view.Class,
                _view.Gender);
        }

        private void CreateCharacterRequest(string username, string characterName, int faction, Class characterClass, Gender gender)
        {
            CreateCharacterRequestMessage createChracterRequestMessage = new CreateCharacterRequestMessage
            {
                Username = username,
                CharacterName = characterName,
                Faction = faction,
                CharacterClass = (int)characterClass,
                Gender = (int)gender,
                MapCode = _model.MapCode,
            };

            NetworkClient.ReplaceHandler<CreateCharacterResponsetMessage>(OnCreateCharacterResultReceived);
            NetworkClient.Send(createChracterRequestMessage);
        }

        private void OnCreateCharacterResultReceived(CreateCharacterResponsetMessage msg)
        {
            switch (msg.Result)
            {
                case CreateCharacterResult.SUCCESS:
                    _view.CreateCharacterSuccess();
                    Managers.UI.OpenAlert("SUCCESS", "The character creation has been completed.");
                    LoadCharacterInfo();
                    _view.OpenPanel("GameStart");
                    break;
                case CreateCharacterResult.DUPLICATE:
                    Managers.UI.OpenAlert("DUPLICATE", "This is a character name that already exists.");
                    break;
                case CreateCharacterResult.ERROR:
                default:
                    Managers.UI.OpenAlert("ERROR", "Error occurred. Error code 101");
                    break;
            }
        }

        /// <summary>
        /// 캐릭터 리스트를 받아와서 구성하는 부분
        /// </summary>
        private void LoadCharacterInfo()
        {
            LoadCharacterInfoRequest();
        }

        private void LoadCharacterInfoRequest()
        {
            CharacterInfoLoadRequestMessage loadCharacterRequestMessage = new CharacterInfoLoadRequestMessage
            {
                Username = Managers.Game.playerData.AccountID
            };

            NetworkClient.ReplaceHandler<CharacterInfoLoadResponseMessage>(OnLoadCharacterInfoResultReceived);
            NetworkClient.Send(loadCharacterRequestMessage);
        }

        private void OnLoadCharacterInfoResultReceived(CharacterInfoLoadResponseMessage msg)
        {
            var firstChapter = _view.ChapterManager.chapters[0];

            if (msg.CharacterData == null || msg.CharacterData.Count == 0)
            {
                _view.ChapterManager.chapters.Clear();
                _view.ChapterManager.chapters.Add(firstChapter);
                _view.ChapterManager.currentChapterIndex = 0;
                _view.ChapterManager.InitializeChapters();
                return;
            }

            //데이터 가공
            foreach (var character in msg.CharacterData)
            {
                Debug.Log($"Name: {character.title}, Level: {character.description}");

                switch (character.characterClass)
                {
                    case 0:
                        character.background = _view.warriorBackground;
                        break;
                    case 1:
                        character.background = _view.mageBackground;
                        break;
                }

                //UnityEvent 초기화
                if (character.onCreate == null)
                    character.onCreate = new UnityEvent();

                if (character.onPlay == null)
                {
                    character.onPlay = new UnityEvent();
                    character.onPlay.AddListener(() =>
                    {
                        //해당 캐릭터에 대한 정보 받아오는 이벤트
                        SelectCharacter(character.title);
                    });

                }

                if (character.onDelete == null)
                {
                    character.onDelete = new UnityEvent();
                    character.onDelete.AddListener(() =>
                    {
                        //캐릭터를 정말 삭제할건지 묻는 팝업을 띄우고 해당 팝업의 Confirm에 삭제 리스너
                        //삭제가 완료된 뒤 창을 닫고, 리스너를 제거함
                        _view.DeleteCharacterPopup.onConfirm.AddListener(() =>
                        {
                            DeleteCharacter(character.title);
                            _view.DeleteCharacterPopup.CloseWindow();
                            _view.DeleteCharacterPopup.onConfirm.RemoveAllListeners();
                        });
                        _view.DeleteCharacterPopup.OpenWindow();
                    });
                }

            }

            // 첫 번째 요소(캐릭터 생성창)를 제외하고 리스트 초기화 후 받아온 정보 추가함.
            if (_view.ChapterManager.chapters.Count > 1)
            {
                _view.ChapterManager.chapters.Clear();
                _view.ChapterManager.chapters.Add(firstChapter);
            }

            _view.ChapterManager.chapters.AddRange(msg.CharacterData);
            _view.ChapterManager.InitializeChapters();
        }

        //캐릭터 선택시 해당 캐릭터에 대한 정보를 받아옴
        private void SelectCharacter(string characterName)
        {
            CharacterDataRequest(characterName);
        }

        private void CharacterDataRequest(string characterName)
        {
            CharacterDataRequestMessage characterDataRequestMessage = new CharacterDataRequestMessage
            {
                Username = characterName
            };

            NetworkClient.ReplaceHandler<CharacterDataResponseMessage>(OnCharacterDataReceived);
            NetworkClient.Send(characterDataRequestMessage);
        }

        private void OnCharacterDataReceived(CharacterDataResponseMessage msg)
        {
            Managers.Game.playerData.Username = msg.Username;
            Managers.Game.playerData.Level = msg.Level;
            Managers.Game.playerData.Hp = msg.Hp;
            Managers.Game.playerData.Mp = msg.Mp;
            Managers.Game.playerData.Hxp = msg.Hxp;
            Managers.Game.playerData.Gold = msg.Gold;
            Managers.Game.playerData.MaxHp = msg.MaxHp;
            Managers.Game.playerData.MaxMp = msg.MaxMp;
            Managers.Game.playerData.Attack = msg.Attack;
            Managers.Game.playerData.Class = msg.Class;
            Managers.Game.playerData.Sp = msg.Sp;
            Managers.Game.playerData.Gender = msg.Gender;
            Managers.Game.playerData.Position = msg.Position;
            Managers.Game.playerData.MapCode = msg.MapCode;
            
            Managers.Game.playerData.InventoryItems.Clear();
            foreach (var itemMessage in msg.InventoryItems)
            {
                Managers.Game.playerData.InventoryItems.Add(new InventoryItem
                {
                    ItemId = itemMessage.ItemId,
                    Amount = itemMessage.Amount,
                    Position = itemMessage.Position
                });
            }
            
            DebugUtils.Log($"Player data loaded: {Managers.Game.playerData.Username}");
            
            //캐릭터 정보를 모두 받아왔으면 인게임으로 씬 전환을 시작
            InitInGame();
        }

        private void InitInGame()
        {
            Managers.UI.OpenAlert("LOADING", "Ingame loading...", 2);
            Managers.Map.LoadInGame();
        }

        //캐릭터 삭제
        private void DeleteCharacter(string characterName)
        {
            DeleteCharacterRequest(characterName);
        }

        private void DeleteCharacterRequest(string characterName)
        {
            DeleteCharacterRequestMessage deleteCharacterRequestMessage = new DeleteCharacterRequestMessage
            {
                Username = characterName
            };

            NetworkClient.ReplaceHandler<DeleteCharacterResponseMessage>(OnDeleteCharacterReceived);
            NetworkClient.Send(deleteCharacterRequestMessage);
        }

        private void OnDeleteCharacterReceived(DeleteCharacterResponseMessage msg)
        {
            if (true == msg.Result)
            {
                LoadCharacterInfo();
            }
        }
        #endregion
    }
}