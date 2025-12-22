using Mirror;
using TMPro;
using UnityEngine;

namespace NOLDA
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

        public override void OnStartServer()
        {
            base.OnStartServer();
            Singleton.Session.AddPlayer(this.gameObject);
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            Singleton.Session.RemovePlayer(this.gameObject);
        }

        #region # HP / Damage

        /// <summary>
        /// 몬스터 공격으로 플레이어가 피해를 입을 때 서버에서 호출.
        /// </summary>
        [Server]
        public void TakeDamage(float damage)
        {
            int intDamage = Mathf.Max(1, Mathf.RoundToInt(damage));
            TargetApplyDamage(connectionToClient, intDamage);
        }

        /// <summary>
        /// targetPRC로 해당 클라이언트에서만 실행 / HP 감소 > 로컬 PlayerDataSO
        /// </summary>
        [TargetRpc]
        private void TargetApplyDamage(NetworkConnection target, int damage)
        {
            var playerData = Singleton.Game?.playerData;

            playerData.Hp -= damage;
            DebugUtils.Log($"{gameObject.name} 피해 {damage}, 현재 HP: {playerData.Hp}/{playerData.TotalMaxHp}");

            // HP 0 이하일때 죽는 로직 추가 필요
            if (playerData.Hp <= 0)
            {
                DebugUtils.Log("사망");
                //사망 UI 및 서버에 전송 필요
            }
        }

        #endregion

        #region # Sync Nickname / Class
        //Sync Nickname
        private void InitNickName()
        {
            CmdSetNickname(Singleton.Game.playerData.Username);
        }

        [Command]
        private void CmdSetNickname(string nickname)
        {
            this.playerNickname = nickname;
            gameObject.name = nickname;
        }

        private void OnNicknameChanged(string oldNickname, string newNickname)
        {
            UpdateNicknameUI(newNickname);
        }

        private void UpdateNicknameUI(string nickname)
        {
            if (nicknameText != null)
            {
                gameObject.name = nickname;
                nicknameText.text = nickname;
            }
        }

        //Sync Class
        private void InitClass()
        {
            CmdSetClass(Singleton.Game.playerData.Class);
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
            var avatarPrefab = Singleton.Game.GetAvatarPrefab(this.playerClass);

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
            transform.position = Singleton.Game.playerData.Position;
            _characterController.enabled = true;

            SavePosition().Forget(); // 위치 지속적으로 저장
        }

        private async Awaitable SavePosition()
        {
            while (true)
            {
                await Awaitable.WaitForSecondsAsync(5f);
                if (this == null)
                {
                    break;
                }
                Singleton.Game.playerData.Position = transform.position;
            }
        }
        #endregion

        /// <summary>
        /// 플레이어 삭제 요청(타이틀로 나갈 때)
        /// </summary>
        [Command(requiresAuthority = false)]
        public void CmdRemovePlayer()
        {
            Singleton.Session.RemovePlayer(this.gameObject);
            NetworkServer.Destroy(connectionToClient.identity.gameObject);
            NetworkServer.RemovePlayerForConnection(connectionToClient);
        }
    }
}