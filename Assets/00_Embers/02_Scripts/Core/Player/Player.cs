using Mirror;
using TMPro;
using UnityEngine;

namespace STARTING
{
    public class Player : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnNicknameChanged))]
        public string playerNickname;

        [SyncVar(hook = nameof(OnClassChanged))]
        public Class playerClass = Class.NONE;
        
        [Header("플레이어 아바타가 바인드될 곳")]
        public Transform playerAvatarBind;
        [Header("캐릭터 닉네임")]
        public TMP_Text nicknameText;
        
        private CharacterController _characterController;
        private Animator _animator;
        [HideInInspector]
        public bool lockCursor = false;

        private void Awake()
        {
            TryGetComponent<CharacterController>(out _characterController);
            TryGetComponent<Animator>(out _animator);
        }
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            
            InitNickName();
            InitializePlayerPosition();
            InitClass(); //Class SyncVar로 공유 후 Init Avatar 해줌.
        }

        #region # Sync Nickname / Class
        //Sync Nickname
        private void InitNickName()
        {
            CmdSetNickname(Managers.Game.playerData.Username);
        }
        
        [Command]
        private void CmdSetNickname(string nickname)
        {
            this.playerNickname = nickname;
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
        
        //Sync Class
        private void InitClass()
        {
            CmdSetClass(Managers.Game.playerData.Class);
        }
        
        [Command(requiresAuthority = false)]
        public void CmdSetClass(Class playerClass)
        {
            this.playerClass = playerClass;
        }

        private void OnClassChanged(Class oldClass, Class newClass)
        {
            InitializePlayerAvatar();
        }
        
        public void InitializePlayerAvatar()
        {
            var avatarPrefab = Managers.Game.GetAvatarPrefab(this.playerClass);
            
            GameObject _currentAvatar = Instantiate(avatarPrefab, playerAvatarBind);
            _currentAvatar.transform.localPosition = Vector3.zero;
            _currentAvatar.transform.localRotation = Quaternion.identity;
            
            _animator.Rebind(); // Animator 초기화
            _animator.Update(0);
        }
        #endregion
        
        #region # Player Position Setting && Save

        private void InitializePlayerPosition()
        {
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