using UnityEngine;

namespace NOLDA
{
    public class ServerConnect : MonoBehaviour
    {
        public TitleUIController controller;

        private void OnEnable()
        {
            TryConnect();
        }

        public void TryConnect()
        {
            controller.ServerConnect();
        }
    }
}