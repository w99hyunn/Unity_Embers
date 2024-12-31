using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace STARTING
{
    public class CharacterUIController : MonoBehaviour
    {
        [Header("Title UI - Manager")]
        public TitleUIManager TitleUIManager;

        private CharacterUIView _view;
        private CharacterUIModel _model;

        private void Awake()
        {
            TryGetComponent<CharacterUIView >(out _view);
            TryGetComponent<CharacterUIModel>(out _model);
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
                mapCode = _model.MapCode,
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
                    TitleUIManager.Alert("SUCCESS", "The character creation has been completed.");
                    LoadCharacterInfo();
                    TitleUIManager.OpenPanel("GameStart");
                    break;
                case CreateCharacterResult.DUPLICATE:
                    TitleUIManager.Alert("DUPLICATE", "This is a character name that already exists.");
                    break;
                case CreateCharacterResult.ERROR:
                default:
                    TitleUIManager.Alert("ERROR", "Error occurred. Error code 101");
                    break;
            }
        }

        public void LoadCharacterInfo()
        {
            LoadCharacterInfoRequest();
        }

        private void LoadCharacterInfoRequest()
        {
            CharacterInfoLoadRequestMessage loadCharacterRequestMessage = new CharacterInfoLoadRequestMessage
            {
                username = Managers.Game.playerData.AccountID
            };

            NetworkClient.ReplaceHandler<CharacterInfoLoadResponseMessage>(OnLoadCharacterInfoResultReceived);
            NetworkClient.Send(loadCharacterRequestMessage);
        }

        private void OnLoadCharacterInfoResultReceived(CharacterInfoLoadResponseMessage msg)
        {
            //데이터 가공
            foreach (var character in msg.characterData)
            {
                Debug.Log($"Name: {character.title}, Level: {character.description}");

                switch (character.characterClass)
                {
                    case 0:
                        character.background = _view.warriorBackground;
                        break;
                    case 1:
                        character.background = _view.mageBackground;
                        break;
                }

                //UnityEvent 초기화
                if (character.onCreate == null)
                    character.onCreate = new UnityEvent();

                if (character.onPlay == null)
                    character.onPlay = new UnityEvent();//TODO 여기에 해당 캐릭터 실행 로직 추가해야함

                if (character.onDelete == null)
                    character.onDelete = new UnityEvent(); //TODO 캐릭터 삭제 실행 로직 추가 요함

            }

            // 첫 번째 요소를 제외하고 리스트 초기화
            if (TitleUIManager.chapterManager.chapters.Count > 1)
            {
                var firstChapter = TitleUIManager.chapterManager.chapters[0];
                TitleUIManager.chapterManager.chapters.Clear();
                TitleUIManager.chapterManager.chapters.Add(firstChapter);
            }

            // 새로운 데이터 추가
            TitleUIManager.chapterManager.chapters.AddRange(msg.characterData);
            TitleUIManager.chapterManager.InitializeChapters();
        }
    }
}