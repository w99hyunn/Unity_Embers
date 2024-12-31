using Mirror;
using UnityEngine;

public enum Class
{
    WARRIOR = 0,
    MAGE = 1,
}

public enum Gender
{
    MALE = 0,
    FEMALE = 1,
}

namespace STARTING
{
    public class CreateCharacterController : MonoBehaviour
    {
        private CreateCharacterView _view;
        public int defaultMapCode = 0001;

        private void Awake()
        {
            TryGetComponent<CreateCharacterView >(out _view);
        }

        public void CreateCharacter()
        {
            CreateCharacterRequest(
                Managers.Game.playerData.AccountID,
                _view.Name,
                _view.Faction,
                _view.Class,
                _view.Gender);
        }

        private void CreateCharacterRequest(string username, string characterName, int faction, Class characterClass, Gender gender)
        {
            CreateCharacterRequestMessage createChracterRequestMessage = new CreateCharacterRequestMessage
            {
                username = username,
                characterName = characterName,
                faction = faction,
                characterClass = (int)characterClass,
                gender = (int)gender,
                mapCode = defaultMapCode,
            };

            NetworkClient.ReplaceHandler<CreateCharacterResponsetMessage>(OnCreateCharacterResultReceived);
            NetworkClient.Send(createChracterRequestMessage);
        }

        private void OnCreateCharacterResultReceived(CreateCharacterResponsetMessage msg)
        {
            switch (msg.result)
            {
                case CreateCharacterResult.SUCCESS:
                    _view.CreateCharacterSuccess();
                    break;
                case CreateCharacterResult.DUPLICATE:
                    _view.Alert("DUPLICATE", "This is a character name that already exists.");
                    break;
                case CreateCharacterResult.ERROR:
                default:
                    _view.Alert("ERROR", "Error occurred. Error code 101");
                    break;
            }
        }
    }
}