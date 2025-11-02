using System.Runtime.InteropServices;
using UnityEngine;

namespace NOLDA
{
    public class CapslockPressed : MonoBehaviour
    {
        private CanvasGroup canvasGroup;
        
        [DllImport("user32.dll")]
        private static extern short GetKeyState(int keyCode);
        private const int VK_CAPSLOCK = 0x14; // Caps Lock
        
        private void Awake()
        {
            TryGetComponent<CanvasGroup>(out canvasGroup);
            canvasGroup.alpha = 0f;
        }

        public void OnChatInputChanged()
        {
            UpdateCanvasGroup();
        }
        
        private void UpdateCanvasGroup()
        {
            // Caps Lock이 켜져있으면 CanvasGroup의 alpha를 1로 설정
            if (IsCapsLockOn())
            {
                canvasGroup.alpha = 1f;
            }
            else
            {
                canvasGroup.alpha = 0f;
            }
        }
        
        private bool IsCapsLockOn()
        {
            // GetKeyState를 사용해 Caps Lock 상태를 반환 (0x0001 비트 확인)
            return (GetKeyState(VK_CAPSLOCK) & 0x0001) != 0;
        }
    }
}