using UnityEngine;
using UnityEngine.UI;

namespace NOLDA
{
    public class ClassBackgroundChanger : MonoBehaviour
    {
        private Image _backgroundImage;

        private void Awake()
        {
            TryGetComponent<Image>(out _backgroundImage);
        }

        public void BackgroundChange(Sprite sprite)
        {
            _backgroundImage.sprite = sprite;
        }
    }
}