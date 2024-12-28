using Cysharp.Threading.Tasks;
using Mirror;
using UnityEngine;

namespace STARTING
{
    public class TitleUIController : MonoBehaviour
    {
        private TitleUIView _view;

        private int serverConnectMaxRetry = 10;

        private void Start()
        {
            TryGetComponent<TitleUIView>(out _view);
        }

        public async void ServerConnect()
        {
            await WaitForNetworkInitialization();
            CheckConnectionAsync().Forget();
        }

        private async UniTask WaitForNetworkInitialization()
        {
            while (Managers.Network == null)
            {
                await UniTask.Yield();
            }
        }

        /// <summary>
        /// 접속시 서버 연결
        /// </summary>
        /// <returns></returns>
        private async UniTask CheckConnectionAsync()
        {
            int attemptCount = 0;
            bool connected = false;

            while (attemptCount < serverConnectMaxRetry && !connected)
            {
                Managers.Network.StartClient();
                attemptCount++;

                _view.ConnectingMessageUpdate($"Connect Try {attemptCount} / {serverConnectMaxRetry}");

                //연결됐는지 확인
                while (NetworkClient.active && !NetworkClient.isConnected)
                {
                    await UniTask.Yield();
                }

                // 연결 성공
                if (NetworkClient.isConnected)
                {
                    _view.ConnectingSuccess();
                    connected = true;
                }
                else
                {
                    await UniTask.Delay(2000);
                }
            }

            // 10번 재시도 후에도 연결되지 않으면 실패
            if (!connected)
            {
                _view.ConnectingFail();
            }
        }

        /// <summary>
        /// 회원가입
        /// </summary>
        public void SignUp()
        {
            SignUpRequest(_view.signUpIdInputField.text, _view.signUpPwInputField.text, _view.signUpEmailInputField.text);
        }

        private void SignUpRequest(string username, string password, string email)
        {
            SignUpRequestMessage signUpRequestMessage = new SignUpRequestMessage
            {
                username = username,
                password = password,
                email = email
            };

            NetworkClient.RegisterHandler<SignUpResponseMessage>(OnSignUpResultReceived);
            NetworkClient.Send(signUpRequestMessage);
        }

        private void OnSignUpResultReceived(SignUpResponseMessage msg)
        {
             _view.SignUpResultPopup(msg.result);
        }

        /// <summary>
        /// 로그인
        /// </summary>
        public void Login()
        {
            LoginRequest(_view.loginIdInputField.text, _view.loginPwInputField.text);
        }

        private void LoginRequest(string username, string password)
        {
            LoginRequestMessage loginRequestMessage = new LoginRequestMessage
            {
                username = username,
                password = password
            };

            NetworkClient.RegisterHandler<LoginResponseMessage>(OnLoginResultReceived);
            NetworkClient.Send(loginRequestMessage);
        }

        private void OnLoginResultReceived(LoginResponseMessage msg)
        {
            _view.LoginResultPopup(msg.result);
        }

    }
}