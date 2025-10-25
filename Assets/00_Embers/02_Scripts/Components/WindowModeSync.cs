using Michsky.UI.Reach;
using UnityEngine;

namespace NOLDA
{
    [RequireComponent(typeof(HorizontalSelector))]
    public class WindowModeSync : MonoBehaviour
    {
        [SerializeField] private HorizontalSelector windowModeSelector;

        void Start()
        {
            UpdateWindowModeSelector();
        }

        void OnEnable()
        {
            UpdateWindowModeSelector();
        }

        void OnDisable()
        {
            UpdateWindowModeSelector();
        }

        private void UpdateWindowModeSelector()
        {
            switch (Screen.fullScreenMode)
            {
                case FullScreenMode.FullScreenWindow:
                    windowModeSelector.index = 0;
                    break;
                case FullScreenMode.ExclusiveFullScreen:
                    windowModeSelector.index = 1;
                    break;
                case FullScreenMode.Windowed:
                    windowModeSelector.index = 2;
                    break;
            }

            windowModeSelector.UpdateUI();
        }
    }
}