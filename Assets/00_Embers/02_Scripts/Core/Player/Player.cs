using Mirror;
using TMPro;
using UnityEngine;

namespace STARTING
{
    public class Player : NetworkBehaviour
    {
        private CharacterController _characterController;
        
        [SyncVar(hook = nameof(OnNicknameChanged))]
        public string playerNickname;
        public TMP_Text nicknameText;
        
        private void Awake()
        {
            TryGetComponent<CharacterController>(out _characterController);
        }
        
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            InitializePlayerPosition();
        }

        private async void InitializePlayerPosition()
        {
            InitNickName();
            
            _characterController.enabled = false;
            transform.position = Managers.Game.playerData.Position;
            Debug.Log($"Position set to {transform.position}");
            
            _characterController.enabled = true;
            
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

        private void InitNickName()
        {
            CmdSetNickname(Managers.Game.playerData.Username);
        }
        
        [Command(requiresAuthority = false)]
        public void CmdRemovePlayer()
        {
            // 플레이어 삭제 요청
            NetworkServer.Destroy(connectionToClient.identity.gameObject);
        }

        private void OnNicknameChanged(string oldNickname, string newNickname)
        {
            UpdateNicknameUI(newNickname);
        }
        
        [Command]
        private void CmdSetNickname(string nickname)
        {
            playerNickname = nickname;
        }
        
        private void UpdateNicknameUI(string nickname)
        {
            if (nicknameText != null)
            {
                nicknameText.text = nickname;
            }
        }

    }
}