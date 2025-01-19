using System.Linq;
using Mirror;
using UnityEngine;

namespace STARTING
{
    public class GameSingleton : MonoBehaviour
    {
        public PlayerDataSO playerData;
        public AvatarDataSO avatarData;
        
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
            if (playerData != null)
            {
                playerData.OnDataChanged += HandleDataChanged;
            }
        }

        private void OnDisable()
        {
            if (playerData != null)
            {
                playerData.OnDataChanged -= HandleDataChanged;
            }
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
                Username = playerData.Username,
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