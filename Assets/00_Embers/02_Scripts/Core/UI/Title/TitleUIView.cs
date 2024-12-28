using UnityEngine;
using TMPro;
using Michsky.UI.Reach;
using UnityEngine.Events;

namespace STARTING
{
    public class TitleUIView : MonoBehaviour
    {
        [Header("Connecting")]
        public GameObject Connecting;
        public TMP_Text ConnectingMessage;
        public GameObject Login;
        public GameObject RetryBtn;
        [Header("Connecting Popup")]
        public ModalWindowManager connectingFailPopup;
        public ModalWindowManager connectingLostPopup;

        [Header("Login")]
        public TMP_InputField loginIdInputField;
        public TMP_InputField loginPwInputField;
        [Header("Login Popup")]
        public ModalWindowManager loginResultPopup;
        public UnityEvent loginSuccess;

        [Header("Sign Up")]
        public TMP_InputField signUpIdInputField;
        public TMP_InputField signUpPwInputField;
        public TMP_InputField signUpEmailInputField;
        [Header("SignUp Popup")]
        public ModalWindowManager signUpPopup;
        public ModalWindowManager signUpResultPopup;

        private string savedLoginId;

        public void ConnectingMessageUpdate(string text)
        {
            ConnectingMessage.text = $"Connecting Server...\r\n{text}";
        }

        public void ConnectingSuccess()
        {
            Connecting.SetActive(false);
            Login.SetActive(true);
        }

        public void ConnectingFail()
        {
            RetryBtn.SetActive(true);
            connectingFailPopup.OpenWindow();
        }

        public void ConnectingLost()
        {
            connectingLostPopup.OpenWindow();
        }

        public void SignUpResultPopup(SignUpResult result)
        {
            string message;

            switch (result)
            {
                case SignUpResult.SUCCESS:
                    message = "Sign Up was successful.";
                    SignUpSuccess();
                    break;
                case SignUpResult.DUPLICATE:
                    message = "This is a duplicate ID. Please use a different ID.";
                    break;
                case SignUpResult.ERROR:
                default:
                    message = "An error has occurred. Please try again.";
                    break;
            }

            signUpResultPopup.windowDescription.text = message;
            signUpResultPopup.OpenWindow();
        }

        private void SignUpSuccess()
        {
            loginIdInputField.text = signUpIdInputField.text;

            signUpIdInputField.text = null;
            signUpPwInputField.text = null;
            signUpEmailInputField.text = null;

            signUpPopup.CloseWindow();
        }

        public void LoginResultPopup(LoginResult result)
        {
            string message;

            switch (result)
            {
                case LoginResult.SUCCESS:
                    LoginSuccess();
                    return;
                case LoginResult.IDWRONG:
                    message = "This ID does not exist.";
                    break;
                case LoginResult.PWWRONG:
                    message = "Password does not match.";
                    break;
                case LoginResult.ERROR:
                default:
                    message = "An error has occurred. Please try again.";
                    break;
            }
            loginResultPopup.windowDescription.text = message;
            loginResultPopup.OpenWindow();
        }

        private void LoginSuccess()
        {
            loginSuccess?.Invoke();
        }

        public void OnSwitchOn()
        {
            LoadLoginId();
            loginIdInputField.onValueChanged.AddListener(SaveLoginId);
        }

        public void OnSwitchOff()
        {
            loginIdInputField.onValueChanged.RemoveListener(SaveLoginId);
        }

        private void SaveLoginId(string value)
        {
            savedLoginId = value;
            PlayerPrefs.SetString("LoginId", savedLoginId);
            PlayerPrefs.Save();
        }

        private void LoadLoginId()
        {
            if (PlayerPrefs.HasKey("LoginId"))
            {
                savedLoginId = PlayerPrefs.GetString("LoginId");
                loginIdInputField.text = savedLoginId;
            }
        }
    }
}