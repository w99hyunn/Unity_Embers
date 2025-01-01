using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace STARTING
{
    public class GameManager : MonoBehaviour
    {
        //Managers.Game.playerData의 정보가 업데이트되면 자동으로 네트워크 메시지를 보내 DB 업데이트
        public PlayerDataSO playerData;

        [Header("K를 누르면 아래에 해당 정보를 업데이트하는용으로 테스트할 수 있음.")]
        public string testCharacterName;
        public int testHP;
        public int testMaxHP;
        public int testMP;
        public int testMaxMP;
        public Vector3 testPosition; 

        private void OnTest1(InputValue input)
        {
            playerData.Username = testCharacterName;
            playerData.Hp = testHP;
            playerData.MaxHp = testMaxHP;
            playerData.Mp = testMP;
            playerData.MaxMp = testMaxMP;
            playerData.Position = testPosition;
        }



        public void SelectCharacterAfterDataUpdateEventRegister()
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
            Debug.Log($"Field {fieldName} changed to {newValue}");
            SendDataToServer(fieldName, newValue);
        }

        private void SendDataToServer(string fieldName, object newValue)
        {
            UpdatePlayerDataMessage message = new UpdatePlayerDataMessage
            {
                username = playerData.Username,
                fieldName = fieldName,
                newValue = newValue.ToString()
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

        public void InitCharacter()
        {

        }
    }
}