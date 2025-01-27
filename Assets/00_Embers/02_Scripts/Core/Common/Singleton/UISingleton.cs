using Michsky.UI.Reach;
using UnityEngine;

namespace NOLDA
{
    public class UISingleton : MonoBehaviour
    {
        [Header("Common Alert Popup")]
        public ModalWindowManager alertModal;

        [Header("Location Alert")]
        public FeedNotification localtionNoti;
        
        [Header("Fade Screen")]
        public ImageFading initPanel;

        #region # Alert
        /// <summary>
        /// 팝업
        /// </summary>
        /// <param name="title">제목</param>
        /// <param name="message">내용</param>
        /// <param name="buttonState">0 = Confirm, 1 = Exit Only(Cancel), 2 = No Buttons</param>
        public void OpenAlert(string title, string message, int buttonState = 0)
        {
            alertModal.ExitButtonSet(buttonState); // 
            alertModal.windowTitle.text = title;
            alertModal.windowDescription.text = message;
            alertModal.OpenWindow();
        }

        public void CloseAlert()
        {
            alertModal.CloseWindow();
        }
        #endregion

        public void LocationNoti(string message)
        {
            localtionNoti.notificationText = message;
            localtionNoti.ExpandNotification();
        }
        
        #region # Fade In / Out
        public void FadeIn()
        {
            initPanel.FadeIn();
        }
        
        public void FadeOut()
        {
            _ = StartInitialize();
        }
        
        private async Awaitable StartInitialize()
        {
            await Awaitable.WaitForSecondsAsync(0.5f);
            if (initPanel != null) { initPanel.FadeOut(); }
        }
        #endregion
    }
}