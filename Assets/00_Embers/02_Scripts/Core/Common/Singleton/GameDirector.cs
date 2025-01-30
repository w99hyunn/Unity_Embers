using System.Linq;
using Mirror;
using UnityEngine;

namespace NOLDA
{
    public class GameDirector : MonoBehaviour
    {
        public PlayerDataSO playerData;
        public AvatarDataSO avatarData;

        #region ▶ Game Settings
        [Space(20)]
        [Header("▶ Game Settings")]
        [Header("Charcter Settings - 레벨업시 증가하는 스탯")]
        [SerializeField] private int levelUpAttack = 5;
        public int LevelUpAttack => levelUpAttack;
        [SerializeField] private int levelUpArmor = 10;
        public int LevelUpArmor => levelUpArmor;
        [SerializeField] private int levelUpMaxHp = 100;
        public int LevelUpMaxHp => levelUpMaxHp;
        [SerializeField] private int levelUpMaxMp = 100;
        public int LevelUpMaxMp => levelUpMaxMp;
        [SerializeField] private int levelUpSp = 3;
        public int LevelUpSp => levelUpSp;


        [Header("Server Info")]
        [SerializeField] private string serverIp = "172.30.1.67";
        public string ServerIP => serverIp;
        [SerializeField] private ushort serverPort = 8585;
        public ushort ServerPort => serverPort;

        [SerializeField] private bool serverAutoRun = false;
        public bool ServerAutoRun => serverAutoRun;
        
        [Header("DB Server Info")]
        [SerializeField] private string dbServerIP = "localhost";
        public string DBServerIP => dbServerIP;
        [SerializeField] private string dbHost = "root";
        public string DBHost => dbHost;
        [SerializeField] private string dbPw = "root";
        public string DBPw => dbPw;
        
        
        [Header("InGame")]
        [Tooltip("인벤토리에서 최대치로 쓸 슬롯 갯수. 이 수만큼 빈 슬롯이 생성됨")]
        [SerializeField] private int maxInventorySpace = 60;
        public int MaxInventorySpace => maxInventorySpace;
        [SerializeField] private int chatMaxMessages = 50;
        public int ChatMaxMessages => chatMaxMessages;
        
        
        [Header("Map")]
        [Tooltip("청크 사이즈(각 씬의 맵 사이즈)")]
        [SerializeField] private float chunkSize = 300f;
        public float ChunkSize => chunkSize;
        
        [Tooltip("로드할 주변 청크 수(1로 설정시 플레이어가 있는 청크 기준 1칸 주변까지")]
        [SerializeField] private int loadRange = 1;
        public int LoadRange => loadRange;
        #endregion
        
        public GameObject GetAvatarPrefab(Class playerClass)
        {
            var avatarDataPrefab = avatarData.avatarList.FirstOrDefault(data => data.classType == playerClass);
            
            if (avatarDataPrefab != null)
            {
                return avatarDataPrefab.avatarPrefab;
            }
            
            return null;
        }
        
        #region # 캐릭터 데이터 이벤트 등록
        //Managers.Game.playerData의 정보가 업데이트되면 자동으로 네트워크 메시지를 보내 DB 업데이트
        public void OnEnable()
        {
            playerData.OnDataChanged += HandleDataChanged;
        }

        private void OnDisable()
        {
            playerData.OnDataChanged -= HandleDataChanged;
        }
        #endregion
        
        #region # PlayerDataUpdate Logic
        private void HandleDataChanged(string fieldName, object newValue)
        {
            SendDataToServer(fieldName, newValue);
        }

        private void SendDataToServer(string fieldName, object newValue)
        {
            UpdatePlayerDataMessage message = new UpdatePlayerDataMessage
            {
                CharacterName = playerData.Username,
                FieldName = fieldName,
                NewValue = newValue.ToString()
            };

            NetworkClient.Send(message);
        }

        public void LoginSuccess(string accountID, string email, string createdDate)
        {
            playerData.AccountID = accountID;
            playerData.Email = email;
            playerData.CreatedDate = createdDate;
        }

        public void UserInfoUpdate(string email)
        {
            playerData.Email = email;
        }
        #endregion

        #region # Skill Update Logic
        // 네트워크로 skillID와 level을 직접 전송하는 함수
        public void SendSkillUpdateToServer(string characterName, int skillID, int level)
        {
            if (!NetworkClient.active) return;

            UpdateSkillMessage message = new UpdateSkillMessage
            {
                CharacterName = characterName,
                SkillID = skillID,
                Level = level
            };

            NetworkClient.Send(message);
        }
        #endregion

        #region # Inventory Update Logic
        public void SendSlotUpdateToServer(int index)
        {
            var item = playerData.Items[index];

            UpdateInventoryMessage updateInventoryMessage = new UpdateInventoryMessage
            {
                CharacterName = playerData.Username,
                Index = index,
                ItemId = item != null ? item.Data.ID : -1, // 아이템 ID가 없으면 -1
                Amount = item is CountableItem ci ? ci.Amount : 1 // 셀수 있는 아이템처리
            };

            NetworkClient.Send(updateInventoryMessage);
        }
        #endregion
    }
}