using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace NOLDA
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

        #region # General
        public async void ServerConnect()
        {
            await WaitForNetworkInitialization();
            await CheckConnectionAsync();
        }

        private async Awaitable WaitForNetworkInitialization()
        {
            while (Singleton.Network == null)
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
                Singleton.Network.StartClient();
                attemptCount++;

                _view.ConnectingMessageUpdate($"{attemptCount} / {SERVER_CONNECT_MAX_RETRY}");

                //연결됐는지 확인
                while (NetworkClient.active && !NetworkClient.isConnected)
                {
                    await Awaitable.NextFrameAsync();
                }

                // 연결 성공
                if (NetworkClient.isConnected)
                {
                    _view.ConnectingSuccess();
                    CheckServerConnectionLoop().Forget();
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
                Singleton.UI.OpenAlert("연결 실패",
                "연결에 실패했습니다. 계속 연결에 실패하면 홈페이지에서 서버 상태를 확인하세요.");
            }
        }


        private async Awaitable CheckServerConnectionLoop()
        {
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
            Singleton.UI.OpenAlert("연결 종료",
                "연결이 손실되었습니다. 게임을 종료합니다.", 1);
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
                Singleton.UI.OpenAlert("실패", "아이디는 최소 5자 이상이어야 합니다.");
                return;
            }

            //비밀번호 길이 확인
            if (_view.SignUpPw.Length < 5)
            {
                Singleton.UI.OpenAlert("실패", "비밀번호는 최소 5자 이상이어야 합니다.");
                return;
            }

            //비밀번호 제대로 입력했는지 체크
            if (_view.SignUpPw != _view.SignUpPwConfirm)
            {
                Singleton.UI.OpenAlert("실패", "비밀번호가 일치하지 않습니다.");
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

            Singleton.UI.OpenAlert(title, message);
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
                    Singleton.Game.LoginSuccess(msg.Username, msg.Email, msg.CreatedDate);
                    _view.menuManager.DisableSplashScreen();
                    _view.TopPanelProfileUpdate();
                    LoadCharacterInfo();
                    return;
                case LoginResult.IDWRONG:
                    title = "아이디 오류";
                    message = "존재하지 않는 아이디입니다.";
                    break;
                case LoginResult.PWWRONG:
                    title = "비밀번호 오류";
                    message = "비밀번호가 일치하지 않습니다.";
                    break;
                case LoginResult.ERROR:
                default:
                    title = "오류";
                    message = "오류가 발생했습니다. 다시 시도해주세요.";
                    break;

            }

            Singleton.UI.OpenAlert(title, message);
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
                Singleton.UI.OpenAlert("실패", "비밀번호가 일치하지 않습니다.");
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
                Username = Singleton.Game.playerData.AccountID,
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
                Singleton.Game.UserInfoUpdate(msg.Email);
                Singleton.UI.OpenAlert("성공", "회원정보 업데이트가 완료되었습니다.");
                _view.EditProfileUpdateSuccess();
            }
            else
            {
                Singleton.UI.OpenAlert("실패", "회원정보 업데이트에 실패했습니다.");
            }
        }
        #endregion

        #region # Character
        public void CreateCharacter()
        {
            CreateCharacterRequest(
                Singleton.Game.playerData.AccountID,
                _view.Name,
                _view.Faction,
                _view.Class,
                _view.Gender);
        }

        private void CreateCharacterRequest(string username, string characterName, Faction faction, Class characterClass, Gender gender)
        {
            CreateCharacterRequestMessage createChracterRequestMessage = new CreateCharacterRequestMessage
            {
                Username = username,
                CharacterName = characterName,
                Faction = faction,
                CharacterClass = characterClass,
                Gender = gender,
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
                    Singleton.UI.OpenAlert("성공", "캐릭터 생성이 완료되었습니다.");
                    LoadCharacterInfo();
                    _view.OpenPanel("GameStart");
                    break;
                case CreateCharacterResult.DUPLICATE:
                    Singleton.UI.OpenAlert("실패", "이미 존재하는 캐릭터 이름입니다.");
                    break;
                case CreateCharacterResult.ERROR:
                default:
                    Singleton.UI.OpenAlert("실패", "오류가 발생했습니다. 오류코드 101");
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
                Username = Singleton.Game.playerData.AccountID
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
                //Debug.Log($"Name: {character.title}, Level: {character.description}");

                switch (character.characterClass)
                {
                    case Class.WARRIOR:
                        character.background = _view.warriorBackground;
                        break;
                    case Class.MAGE:
                        character.background = _view.mageBackground;
                        break;
                    case Class.ROGUE:
                        character.background = _view.rogueBackground;
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
                            Debug.Log("삭제 눌림 " + character.title);
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
            Singleton.Game.playerData.Init(msg);
            InitInGameAsync().Forget();
        }

        private async Awaitable InitInGameAsync()
        {
            Singleton.UI.FadeIn();
            Singleton.UI.OpenLoading("게임 시작", "탐험을 시작하기 위해 준비중입니다.\n잠시만 기다려주세요.", 3);
            await Awaitable.WaitForSecondsAsync(0.5f);
            Singleton.Map.LoadInGame();
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