using Mirror;
using UnityEngine;

namespace STARTING
{
    public class GeneralUIController : MonoBehaviour
    {
        [Header("Title UI - Manager")]
        public TitleUIManager TitleUIManager;

        private GeneralUIView _view;

        private int serverConnectMaxRetry = 10;
        private bool isCheckingConnection;

        private void Start()
        {
            TryGetComponent<GeneralUIView>(out _view);
        }

        public async void ServerConnect()
        {
            await WaitForNetworkInitialization();
            CheckConnectionAsync();
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
            int attemptCount = 0;
            bool connected = false;

            while (attemptCount < serverConnectMaxRetry && !connected)
            {
                Managers.Network.StartClient();
                attemptCount++;

                _view.ConnectingMessageUpdate($"Try {attemptCount} / {serverConnectMaxRetry}");

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
                TitleUIManager.Alert("CONNECTING FAIL",
                "The server cannot be connected. If you continue to fail to connect, please contact us on the website.");
            }
        }


        private async Awaitable CheckServerConnectionLoop()
        {
            Managers.Log.Log("서버 연결상태 확인 시작");
            isCheckingConnection = true;

            while (isCheckingConnection)
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
            TitleUIManager.Alert("CONNECTING LOST",
                "The connection to the server is lost, and the game is terminated.", true);
            isCheckingConnection = false;
        }

        /// <summary>
        /// 회원가입
        /// </summary>
        public void SignUp()
        {
            if (_view.SignUpPW != _view.SignUpPWConfirm)
            {
                TitleUIManager.Alert("FAIL", "Invalid password input, please enter the same value.");
                return;
            }

            SignUpRequest(_view.SignUpID, _view.SignUpPW, _view.SignUpEmail);
        }

        private void SignUpRequest(string username, string password, string email)
        {
            SignUpRequestMessage signUpRequestMessage = new SignUpRequestMessage
            {
                username = username,
                password = password,
                email = email
            };

            NetworkClient.ReplaceHandler<SignUpResponseMessage>(OnSignUpResultReceived);
            NetworkClient.Send(signUpRequestMessage);
        }

        private void OnSignUpResultReceived(SignUpResponseMessage msg)
        {
            string title;
            string message;

            switch (msg.result)
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

            TitleUIManager.Alert(title, message);
        }

        /// <summary>
        /// 로그인
        /// </summary>
        public void Login()
        {
            LoginRequest(_view.LoginID, _view.LoginPW);
        }

        private void LoginRequest(string username, string password)
        {
            LoginRequestMessage loginRequestMessage = new LoginRequestMessage
            {
                username = username,
                password = password
            };

            NetworkClient.ReplaceHandler<LoginResponseMessage>(OnLoginResultReceived);
            NetworkClient.Send(loginRequestMessage);
        }

        private void OnLoginResultReceived(LoginResponseMessage msg)
        {
            string title;
            string message;

            switch (msg.result)
            {
                case LoginResult.SUCCESS:
                    Managers.Game.LoginSuccess(msg.username, msg.email, msg.createdDate);
                    TitleUIManager.LoginSuccess();
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

            TitleUIManager.Alert(title, message);
        }

        public void EditProfilePopupOpen()
        {
            _view.EditProfilePopupInit();
        }

        public void EditProfileConfirm()
        {
            if (_view.EditProfilePW != _view.EditProfilePWConfirm)
            {
                //뭐라도 값이 입력됐는데 두개 필드가 다르면 안내
                TitleUIManager.Alert("FAIL", "Invalid password input, please enter the same value.");
                return;
            }

            EditProfileUpdate(_view.EditProfilePW, _view.EditProfileEmail);

        }

        public void EditProfileUpdate(string password, string email)
        {
            EditProfileRequest(password, email);
        }

        private void EditProfileRequest(string password, string email)
        {
            ProfileUpdateRequestMessage profileUpdateRequestMessage = new ProfileUpdateRequestMessage
            {
                username = Managers.Game.playerData.AccountID,
                email = email,
                password = password
            };

            NetworkClient.ReplaceHandler<ProfileUpdateResponseMessage>(OnProfileUpdateResultReceived);
            NetworkClient.Send(profileUpdateRequestMessage);
        }

        private void OnProfileUpdateResultReceived(ProfileUpdateResponseMessage msg)
        {
            if (true == msg.success)
            {
                Managers.Game.UserInfoUpdate(msg.email);
                TitleUIManager.Alert("SUCCESS", "User information update successful.");
                _view.EditProfileUpdateSuccess();
            }
            else
            {
                TitleUIManager.Alert("FAIL", "Failed to update user information.");
            }
        }

    }
}