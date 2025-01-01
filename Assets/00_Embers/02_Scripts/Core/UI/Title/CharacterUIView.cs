using Michsky.UI.Reach;
using TMPro;
using UnityEngine;

namespace STARTING
{
    public class CharacterUIView : MonoBehaviour
    {
        public Sprite warriorBackground;
        public Sprite mageBackground;
        public ModalWindowManager deleteCharacterPopup;


        [Space(20)]
        public ModeSelector classSelector;
        public HorizontalSelector genderSelector;
        public HorizontalSelector factionSelector;
        public TMP_InputField nameInputField;

        public Class Class => (Class)classSelector.currentModeIndex;
        public Gender Gender => (Gender)genderSelector.index;
        public int Faction => factionSelector.index;
        public string Name => nameInputField.text;


        public void CreateCharacterSuccess()
        {
            //Reset
            nameInputField.text = "";
        }
    }
}