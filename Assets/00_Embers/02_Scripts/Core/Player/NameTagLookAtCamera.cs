using Mirror;
using UnityEngine;

namespace STARTING
{
    public class NameTagLookAtCamera : MonoBehaviour
    {
        private Transform _cameraTransform;

        private void Start()
        {
            _ = FindLocalPlayer();
        }

        private async Awaitable FindLocalPlayer()
        {
            while (NetworkClient.localPlayer == null)
            {
                await Awaitable.NextFrameAsync();
            }

            _cameraTransform = NetworkClient.localPlayer.GetComponent<PlayerController>().mainCamera.transform;
        }
        
        void Update()
        {
            transform.LookAt(_cameraTransform);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 180f, 0);
        }
    }
}