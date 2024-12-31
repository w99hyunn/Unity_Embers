using Michsky.UI.Reach;
using UnityEngine;

namespace STARTING
{
    public class TitleUIManager : MonoBehaviour
    {
        [Header("General")]
        public ModalWindowManager alertModal;
        public MenuManager menuManager;
        public PanelManager panelManager;

        /// <summary>
        /// General
        /// </summary>
        /// <param name="message"></param>
        public void Alert(string title, string message, bool isExit = false)
        {
            if (true == isExit)
            {
                alertModal.ExitButtonSet(true);
            }
            else
            {
                alertModal.ExitButtonSet(false);
            }

            alertModal.windowTitle.text = title;
            alertModal.windowDescription.text = message;
            alertModal.OpenWindow();
        }

        public void OpenPanel(string panelName)
        {
            panelManager.OpenPanel(panelName);
        }
    }
}