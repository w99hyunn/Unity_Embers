using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace STARTING
{
    public class Player : NetworkBehaviour
    {
        public CharacterController characterController;
        public GameObject camera;
        public PlayerInput playerInput;

        private void Start()
        {
            if (false == isLocalPlayer)
            {
                camera.SetActive(false);
                playerInput.enabled = false;
            }
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            InitializePlayerPosition();
        }

        private async void InitializePlayerPosition()
        {
            characterController.enabled = false;

            transform.position = Managers.Game.playerData.Position;
            Debug.Log($"Position set to {transform.position}");
            
            characterController.enabled = true;

            if (false == isLocalPlayer)
            {
                // 위치 지속적으로 저장
                SavePosition();
            }
        }

        private async Awaitable SavePosition()
        {
            while (true)
            {
                await Awaitable.WaitForSecondsAsync(5f);
                Managers.Game.playerData.Position = transform.position;
            }
        }

        [Command(requiresAuthority = false)]
        public void CmdRemovePlayer()
        {
            // 플레이어 삭제 요청
            NetworkServer.Destroy(connectionToClient.identity.gameObject);
        }
    }
}