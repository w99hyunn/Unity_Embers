using UnityEngine;

namespace STARTING
{
    public class SplashUIView : MonoBehaviour
    {
        public GameObject startingLogo;
        public GameObject embersLogo;

        private void Awake()
        {
            StartingLogo(false);
            EmbersLogo(false);
        }

        public void StartingLogo(bool index)
        {
            startingLogo.SetActive(index);
        }

        public void EmbersLogo(bool index)
        {
            embersLogo.SetActive(index);
        }
    }
}