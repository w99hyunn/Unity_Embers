using UnityEngine;

namespace STARTING
{
    public class ServerConnect : MonoBehaviour
    {
        public TitleUIController _controller;

        private void OnEnable()
        {
            TryConnect();
        }

        public void TryConnect()
        {
            _controller.ServerConnect();
        }
    }
}