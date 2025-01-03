using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace STARTING
{
    public class GameManager : MonoBehaviour
    {
        //Managers.Game.playerData의 정보가 업데이트되면 자동으로 네트워크 메시지를 보내 DB 업데이트
        public PlayerDataSO playerData;

        public bool suppressDataChangeEvents = true;

        [Header("K를 누르면 아래에 해당 정보를 업데이트하는용으로 테스트할 수 있음.")]
        public string testCharacterName;
        public int testHp;
        public int testMaxHp;
        public int testMp;
        public int testMaxMp;
        public Vector3 testPosition; 

        private void OnTest1(InputValue input)
        {
            playerData.Username = testCharacterName;
            playerData.Hp = testHp;
            playerData.MaxHp = testMaxHp;
            playerData.Mp = testMp;
            playerData.MaxMp = testMaxMp;
            playerData.Position = testPosition;
        }



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
            if (false == suppressDataChangeEvents)
            {
                return;
            }

            Debug.Log($"Field {fieldName} changed to {newValue}");
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

    }
}