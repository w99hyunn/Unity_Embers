using UnityEngine;
using TMPro;
using Michsky.UI.Reach;

namespace STARTING
{
    public class GeneralUIView : MonoBehaviour
    {
        [Header("Splash Screen")]
        [Header("Connecting")]
        public GameObject Connecting;
        public TMP_Text ConnectingMessage;
        public GameObject Login;
        public GameObject RetryBtn;

        [Header("Login")]
        public TMP_InputField loginIdInputField;
        public TMP_InputField loginPwInputField;

        [Header("Sign Up")]
        public TMP_InputField signUpIdInputField;
        public TMP_InputField signUpPwInputField;
        public TMP_InputField signUpPwConfirmInputField;
        public TMP_InputField signUpEmailInputField;
        [Header("SignUp Popup")]
        public ModalWindowManager signUpPopup;

        [Space(10)]
        [Header("Main Content")]
        [Header("Edit Profile")]
        public ButtonManager profileEdit;
        public ModalWindowManager editProfilePopup;
        public TMP_InputField editProfilePWInputField;
        public TMP_InputField editProfilePWConfirmInputField;
        public TMP_InputField editProfileEmailInputField;
        public TMP_InputField editProfileCreatedInputField;

        public string SignUpID => signUpIdInputField.text;
        public string SignUpPW => signUpPwInputField.text;
        public string SignUpPWConfirm => signUpPwConfirmInputField.text;
        public string SignUpEmail => signUpEmailInputField.text;

        public string LoginID => loginIdInputField.text;
        public string LoginPW => loginPwInputField.text;

        public string EditProfilePW => editProfilePWInputField.text;
        public string EditProfilePWConfirm => editProfilePWConfirmInputField.text;
        public string EditProfileEmail => editProfileEmailInputField.text;

        private void Update()
        {
            if (loginPwInputField.isFocused ||
                signUpPwInputField.isFocused ||
                signUpPwConfirmInputField.isFocused ||
                editProfilePWInputField.isFocused ||
                editProfilePWConfirmInputField.isFocused)
            {
                Input.imeCompositionMode = IMECompositionMode.Off;
            }
            else
            {
                Input.imeCompositionMode = IMECompositionMode.On;
            }
        }

        public void EditProfileUpdateSuccess()
        {
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
        }


        public void SignUpSuccess()
        {
            loginIdInputField.text = signUpIdInputField.text;
            signUpPopup.CloseWindow();

            //Reset
            signUpIdInputField.text = null;
            signUpPwInputField.text = null;
            signUpPwConfirmInputField.text = null;
            signUpEmailInputField.text = null;
        }

        public void TopPanelProfileUpdate()
        {
            profileEdit.buttonText = Managers.Game.playerData.AccountID;
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