using UnityEngine;
using UnityEngine.UI;

namespace STARTING
{
    public class ClassBackgroundChanger : MonoBehaviour
    {
        private Image backgroundImage;

        private void Awake()
        {
            TryGetComponent<Image>(out backgroundImage);
        }

        public void BackgroundChange(Sprite sprite)
        {
            backgroundImage.sprite = sprite;
        }
    }
}