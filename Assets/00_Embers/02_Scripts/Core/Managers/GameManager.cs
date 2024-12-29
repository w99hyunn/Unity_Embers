using UnityEngine;

namespace STARTING
{
    public class GameManager : MonoBehaviour
    {
        public PlayerDataSO playerData;

        public void LoginSuccess(string accountID)
        {
            playerData.AccountID = accountID;
        }

        public void InitCharacter()
        {

        }
    }
}