using UnityEngine;

namespace STARTING
{
    public class GameManager : MonoBehaviour
    {
        public PlayerDataSO playerData;

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