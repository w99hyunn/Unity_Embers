using Mirror;
using TMPro;
using UnityEngine;

namespace STARTING
{
    public class Player : NetworkBehaviour
    {        
        private CharacterController _characterController;
        private Animator _animator;
        
        [SyncVar(hook = nameof(OnNicknameChanged))]
        public string playerNickname;
        public TMP_Text nicknameText;

        [Header("플레이어 아바타가 바인드될 곳")]
        public Transform playerAvatarBind;
        
        public bool lockCursor = false;

        private void Awake()
        {
            TryGetComponent<CharacterController>(out _characterController);
            TryGetComponent<Animator>(out _animator);
        }

        #region # Player Position Setting && Save
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            InitializePlayerPosition();
            InitializePlayerAvatar();
        }
        
        private void InitializePlayerPosition()
        {
            InitNickName();
            
            _characterController.enabled = false;
            transform.position = Managers.Game.playerData.Position;
            _characterController.enabled = true;
            
            _ = SavePosition(); // 위치 지속적으로 저장
        }

        private async Awaitable SavePosition()
        {
            while (true)
            {
                await Awaitable.WaitForSecondsAsync(5f);
                Managers.Game.playerData.Position = transform.position;
            }
        }
        
        public void InitializePlayerAvatar()
        {
            var avatarPrefab = Managers.Game.GetAvatarPrefab();
            
            Debug.Log(avatarPrefab.name);
            
            GameObject _currentAvatar = Instantiate(avatarPrefab, playerAvatarBind);
            _currentAvatar.transform.localPosition = Vector3.zero;
            _currentAvatar.transform.localRotation = Quaternion.identity;
            
            _animator.Rebind(); // Animator 초기화
            _animator.Update(0);
        }
        #endregion
        
        #region # Nickname Sync
        private void InitNickName()
        {
            CmdSetNickname(Managers.Game.playerData.Username);
        }
        
        [Command]
        private void CmdSetNickname(string nickname)
        {
            playerNickname = nickname;
        }
        
        private void OnNicknameChanged(string oldNickname, string newNickname)
        {
            UpdateNicknameUI(newNickname);
        }
        
        private void UpdateNicknameUI(string nickname)
        {
            if (nicknameText != null)
            {
                nicknameText.text = nickname;
            }
        }
        #endregion
        
        /// <summary>
        /// 플레이어 삭제 요청(타이틀로 나갈 때)
        /// </summary>
        [Command(requiresAuthority = false)]
        public void CmdRemovePlayer()
        {
            NetworkServer.Destroy(connectionToClient.identity.gameObject);
            NetworkServer.RemovePlayerForConnection(connectionToClient);
        }
    }
}