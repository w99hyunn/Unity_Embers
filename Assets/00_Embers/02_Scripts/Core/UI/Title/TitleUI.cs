using Michsky.UI.Reach;
using UnityEngine;

namespace STARTING
{
    public class TitleUI : MonoBehaviour
    {
        [Header("General")]
        public MenuManager menuManager;
        public PanelManager panelManager;
        public ChapterManager chapterManager;
        
        public CharacterUIController characterUIController;
        
        public void OpenPanel(string panelName)
        {
            panelManager.OpenPanel(panelName);
        }

        public void LoginSuccess()
        {
            menuManager.DisableSplashScreen();
            characterUIController.LoadCharacterInfo();
        }
    }
}