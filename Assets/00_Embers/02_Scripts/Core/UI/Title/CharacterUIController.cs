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

        /// <summary>
        /// 캐릭터 리스트를 받아와서 구성하는 부분
        /// </summary>
        /// <param name="msg"></param>
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
                {
                    character.onPlay = new UnityEvent();
                    character.onPlay.AddListener(() =>
                    {
                        //해당 캐릭터에 대한 정보 받아오는 이벤트
                        SelectCharacter(character.title);
                    });

                }
                
                if (character.onDelete == null)
                {
                    character.onDelete = new UnityEvent();
                    character.onDelete.AddListener(() =>
                    {
                        //해당 캐릭터를 정말 삭제할건지 묻는 팝업의 Confirm에 삭제 이벤트를 붙이고 해당 팝업을 열음
                        _view.deleteCharacterPopup.onConfirm.AddListener(() =>
                        {
                            DeleteCharacter(character.title);
                        });
                        _view.deleteCharacterPopup.OpenWindow();
                    });
                }

            }

            // 첫 번째 요소(캐릭터 생성창)를 제외하고 리스트 초기화 후 받아온 정보 추가함.
            if (TitleUIManager.chapterManager.chapters.Count > 1)
            {
                var firstChapter = TitleUIManager.chapterManager.chapters[0];
                TitleUIManager.chapterManager.chapters.Clear();
                TitleUIManager.chapterManager.chapters.Add(firstChapter);
            }

            TitleUIManager.chapterManager.chapters.AddRange(msg.characterData);
            TitleUIManager.chapterManager.InitializeChapters();
        }

        //캐릭터 선택시 해당 캐릭터에 대한 정보를 받아옴
        public void SelectCharacter(string characterName)
        {
            CharacterDataRequest(characterName);
        }

        private void CharacterDataRequest(string characterName)
        {
            CharacterDataRequestMessage characterDataRequestMessage = new CharacterDataRequestMessage
            {
                username = characterName
            };

            NetworkClient.ReplaceHandler<CharacterDataResponseMessage>(OnCharacterDataReceived);
            NetworkClient.Send(characterDataRequestMessage);
        }

        private void OnCharacterDataReceived(CharacterDataResponseMessage msg)
        {
            Managers.Game.playerData.Username = msg.Username;
            Managers.Game.playerData.Level = msg.Level;
            Managers.Game.playerData.Hp = msg.Hp;
            Managers.Game.playerData.Mp = msg.Mp;
            Managers.Game.playerData.Exp = msg.Exp;
            Managers.Game.playerData.Gold = msg.Gold;
            Managers.Game.playerData.MaxHp = msg.MaxHp;
            Managers.Game.playerData.MaxMp = msg.MaxMp;
            Managers.Game.playerData.Attack = msg.Attack;
            Managers.Game.playerData.Class = msg.Class;
            Managers.Game.playerData.Sp = msg.Sp;
            Managers.Game.playerData.Gender = msg.Gender;
            Managers.Game.playerData.Position = msg.Position;
            Managers.Game.playerData.MapCode = msg.MapCode;
            Managers.Log.Log($"Player data loaded: {Managers.Game.playerData.Username}");

            //클라이언트 정보를 받아오는걸 성공하면 클라이언트 데이터가 업데이트 될 때마다 서버 DB 업데이트하는 이벤트 등록
            Managers.Game.SelectCharacterAfterDataUpdateEventRegister();
        }


        //캐릭터 삭제
        public void DeleteCharacter(string characterName)
        {
            DeleteCharacterRequest(characterName);
        }

        private void DeleteCharacterRequest(string characterName)
        {
            DeleteCharacterRequestMessage deleteCharacterRequestMessage = new DeleteCharacterRequestMessage
            {
                username = characterName
            };

            NetworkClient.ReplaceHandler<DeleteCharacterResponseMessage>(OnDeleteCharacterReceived);
            NetworkClient.Send(deleteCharacterRequestMessage);
        }

        private void OnDeleteCharacterReceived(DeleteCharacterResponseMessage msg)
        {
            if (true == msg.result)
            {
                LoadCharacterInfo();
            }
        }
    }
}