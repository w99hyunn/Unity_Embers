using UnityEngine;

namespace NOLDA
{
    public class FrameCounter : MonoBehaviour
    {
        public void ShowFPSCounter()
        {
            WaitSingletonSetFPSCounter(true).Forget();
        }

        public void HideFPSCounter()
        {
            WaitSingletonSetFPSCounter(false).Forget();
        }

        private async Awaitable WaitSingletonSetFPSCounter(bool index)
        {
            while (Singleton.UI == null)
            {
                await Awaitable.NextFrameAsync();
            }

            if (index)
            {
                Singleton.UI.ShowFPSCounter();
            }
            else
            {
                Singleton.UI.HideFPSCounter();
            }
        }
    }
}