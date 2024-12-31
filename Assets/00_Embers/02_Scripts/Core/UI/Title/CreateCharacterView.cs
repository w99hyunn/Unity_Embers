using Michsky.UI.Reach;
using TMPro;
using UnityEngine;

namespace STARTING
{
    public class CreateCharacterView : MonoBehaviour
    {
        [Header("Title UI - Manager")]
        public TitleUIManager TitleUIManager;

        [Space(20)]
        public ModeSelector classSelector;
        public HorizontalSelector genderSelector;
        public HorizontalSelector factionSelector;
        public TMP_InputField nameInputField;

        public Class Class => (Class)classSelector.currentModeIndex;
        public Gender Gender => (Gender)genderSelector.index;
        public int Faction => factionSelector.index;
        public string Name => nameInputField.text;

        public void Alert(string title, string message)
        {
            TitleUIManager.Alert(title, message);
        }

        public void OpenPanel(string panelName)
        {
            TitleUIManager.OpenPanel(panelName);
        }

        public void CreateCharacterSuccess()
        {
            Alert("SUCCESS", "The character creation has been completed.");
            OpenPanel("GameStart");

            //reset
            nameInputField.text = "";
        }
    }
}