using UnityEngine;

namespace STARTING
{
    public class ServerConnect : MonoBehaviour
    {
        public TitleGeneralUIController controller;

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