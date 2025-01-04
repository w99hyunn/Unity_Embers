using Michsky.UI.Reach;
using UnityEngine;

namespace STARTING
{
    public class UIManager : MonoBehaviour
    {
        public ModalWindowManager alertModal;
        
        /// <summary>
        /// 팝업
        /// </summary>
        /// <param name="title">제목</param>
        /// <param name="message">내용</param>
        /// <param name="buttonState">0 = Confirm, 1 = Exit Only(Cancel), 2 = No Buttons</param>
        public void Alert(string title, string message, int buttonState = 0)
        {
            alertModal.ExitButtonSet(buttonState); // 
            alertModal.windowTitle.text = title;
            alertModal.windowDescription.text = message;
            alertModal.OpenWindow();
        }
    }
}