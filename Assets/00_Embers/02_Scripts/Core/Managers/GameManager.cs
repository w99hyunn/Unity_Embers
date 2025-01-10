using Mirror;
using UnityEngine;

namespace STARTING
{
    public class GameManager : MonoBehaviour
    {
        //Managers.Game.playerData의 정보가 업데이트되면 자동으로 네트워크 메시지를 보내 DB 업데이트
        public PlayerDataSO playerData;

        #region #PlayerDataUpdate Logic
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
    }
}