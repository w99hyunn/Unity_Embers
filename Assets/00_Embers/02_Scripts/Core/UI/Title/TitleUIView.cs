using UnityEngine;
using TMPro;
using Michsky.UI.Reach;
using UnityEngine.Events;

namespace STARTING
{
    public class TitleUIView : MonoBehaviour
    {
        [Header("General")]
        public ModalWindowManager alertModal;

        [Space(20)]
        [Header("Splash Screen")]
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

        [Space(10)]
        [Header("Main Content")]
        public ButtonManager profileEdit;
        [Header("Edit Profile")]
        public ModalWindowManager editProfilePopup;
        public TMP_InputField editProfilePWInputField;
        public TMP_InputField editProfilePWConfirmInputField;
        public TMP_InputField editProfileEmailInputField;
        public TMP_InputField editProfileCreatedInputField;

        /// <summary>
        /// General
        /// </summary>
        /// <param name="message"></param>
        public void Alert(string message)
        {
            alertModal.windowDescription.text = message;
            alertModal.OpenWindow();
        }

        public void EditProfileUpdateSuccess(string message)
        {
            Alert(message);
            editProfilePopup.CloseWindow();
        }


        /// <summary>
        /// Connecting 관련
        /// </summary>
        /// <param name="text"></param>
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


        /// <summary>
        /// 회원가입 관련
        /// </summary>
        /// <param name="result"></param>
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

        /// <summary>
        /// 로그인 관련
        /// </summary>
        /// <param name="message"></param>
        public void LoginResultPopup(string message)
        {
            loginResultPopup.windowDescription.text = message;
            loginResultPopup.OpenWindow();
        }

        public void LoginSuccess()
        {
            loginSuccess?.Invoke();
            TopPanelProfileUpdate();
        }


        /// <summary>
        /// 스플래시 - 로그인 화면
        /// </summary>
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
            PlayerPrefs.SetString("LoginId", value);
            PlayerPrefs.Save();
        }

        private void LoadLoginId()
        {
            if (PlayerPrefs.HasKey("LoginId"))
            {
                loginIdInputField.text = PlayerPrefs.GetString("LoginId");
            }
        }

        public void TopPanelProfileUpdate()
        {
            profileEdit.buttonText = Managers.Game.playerData.AccountID;
        }

        public void EditProfilePopupInit()
        {
            editProfilePWInputField.text = "";
            editProfilePWConfirmInputField.text = "";
            editProfileEmailInputField.text = Managers.Game.playerData.Email;
            editProfileCreatedInputField.text = Managers.Game.playerData.CreatedDate.ToString();
            editProfilePopup.OpenWindow();
        }


    }
}