using Michsky.UI.Reach;
using UnityEngine;

namespace NOLDA
{
    public class UISingleton : MonoBehaviour
    {
        [Header("Common Alert Popup")]
        public ModalWindowManager alertModal;

        [Header("Loading Screen")]
        public ModalWindowManager loadingScreenModal;

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
            alertModal.ExitButtonSet(buttonState);
            alertModal.windowTitle.text = title;
            alertModal.windowDescription.text = message;
            alertModal.OpenWindow();
        }

        public void CloseAlert()
        {
            alertModal.CloseWindow();
        }
        #endregion

        #region # Loading
        public void OpenLoading(string title, string message, int buttonState = 0)
        {
            loadingScreenModal.ExitButtonSet(buttonState);
            loadingScreenModal.windowTitle.text = title;
            loadingScreenModal.windowDescription.text = message;
            loadingScreenModal.OpenWindow();
        }

        public void CloseLoading()
        {
            loadingScreenModal.CloseWindow();
        }
        #endregion

        #region # Fade In / Out
        public void FadeIn()
        {
            initPanel.FadeIn();
        }
        
        public void FadeOut()
        {
            StartInitialize().Forget();
        }
        
        private async Awaitable StartInitialize()
        {
            await Awaitable.WaitForSecondsAsync(0.5f);
            if (initPanel != null) { initPanel.FadeOut(); }
        }
        #endregion
    }
}