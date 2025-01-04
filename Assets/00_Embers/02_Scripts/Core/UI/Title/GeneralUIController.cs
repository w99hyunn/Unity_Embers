using Mirror;
using UnityEngine;

namespace STARTING
{
    public class GeneralUIController : MonoBehaviour
    {
        [Header("Title UI - Manager")]
        public TitleUI titleUI;

        private GeneralUIView _view;

        private const int SERVER_CONNECT_MAX_RETRY = 10;
        private bool _isCheckingConnection;

        private void Start()
        {
            TryGetComponent<GeneralUIView>(out _view);
        }

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
                    CheckServerConnectionLoop();
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
                Managers.UI.Alert("CONNECTING FAIL",
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
            Managers.UI.Alert("CONNECTING LOST",
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
                Managers.UI.Alert("FAIL", "ID must be at least 5 characters long.");
                return;
            }
            
            //비밀번호 길이 확인
            if (_view.SignUpPw.Length < 5)
            {
                Managers.UI.Alert("FAIL", "Password must be at least 5 characters long.");
                return;
            }
            
            //비밀번호 제대로 입력했는지 체크
            if (_view.SignUpPw != _view.SignUpPwConfirm)
            {
                Managers.UI.Alert("FAIL", "Invalid password input, please enter the same value.");
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

            Managers.UI.Alert(title, message);
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
                    titleUI.LoginSuccess();
                    _view.TopPanelProfileUpdate();
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

            Managers.UI.Alert(title, message);
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
                Managers.UI.Alert("FAIL", "Invalid password input, please enter the same value.");
                return;
            }

            EditProfileUpdate(_view.EditProfilePw, _view.EditProfileEmail);

        }

        public void EditProfileUpdate(string password, string email)
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
                Managers.UI.Alert("SUCCESS", "User information update successful.");
                _view.EditProfileUpdateSuccess();
            }
            else
            {
                Managers.UI.Alert("FAIL", "Failed to update user information.");
            }
        }

    }
}