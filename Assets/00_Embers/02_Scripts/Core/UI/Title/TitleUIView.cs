using UnityEngine;
using TMPro;
using Michsky.UI.Reach;

namespace STARTING
{
    public class TitleUIView : MonoBehaviour
    {
        [Header("General")]
        public MenuManager menuManager;
        public PanelManager panelManager;
        public ChapterManager chapterManager;
        
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

        [Header("Character Select - Class Background")]
        public Sprite warriorBackground;
        public Sprite mageBackground;

        [Space(20)]
        [Header("Character Select - Character Delete Popup")]
        public ModalWindowManager deleteCharacterPopup;

        [Space(20)]
        public ModeSelector classSelector;
        public HorizontalSelector genderSelector;
        public HorizontalSelector factionSelector;
        public TMP_InputField nameInputField;

        #region #GetVar
        public string SignUpID => signUpIdInputField.text;
        public string SignUpPw => signUpPwInputField.text;
        public string SignUpPwConfirm => signUpPwConfirmInputField.text;
        public string SignUpEmail => signUpEmailInputField.text;

        public string LoginID => loginIdInputField.text;
        public string LoginPw => loginPwInputField.text;

        public string EditProfilePw => editProfilePWInputField.text;
        public string EditProfilePwConfirm => editProfilePWConfirmInputField.text;
        public string EditProfileEmail => editProfileEmailInputField.text;
        

        public Class Class => (Class)classSelector.currentModeIndex;
        public Gender Gender => (Gender)genderSelector.index;
        public int Faction => factionSelector.index;
        public string Name => nameInputField.text;
        public ModalWindowManager DeleteCharacterPopup => deleteCharacterPopup;
        public ChapterManager ChapterManager => chapterManager;
        #endregion
        
        public void OpenPanel(string panelName)
        {
            panelManager.OpenPanel(panelName);
        }
        
        public void CreateCharacterSuccess()
        {
            //Reset
            nameInputField.text = "";
        }

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