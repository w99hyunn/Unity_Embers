using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace STARTING
{
    public class CharacterUIController : MonoBehaviour
    {
        [Header("Title UI - Manager")]
        public TitleUI titleUI;

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
                Username = username,
                CharacterName = characterName,
                Faction = faction,
                CharacterClass = (int)characterClass,
                Gender = (int)gender,
                MapCode = _model.MapCode,
            };

            NetworkClient.ReplaceHandler<CreateCharacterResponsetMessage>(OnCreateCharacterResultReceived);
            NetworkClient.Send(createChracterRequestMessage);
        }

        private void OnCreateCharacterResultReceived(CreateCharacterResponsetMessage msg)
        {
            switch (msg.Result)
            {
                case CreateCharacterResult.SUCCESS:
                    _view.CreateCharacterSuccess();
                    Managers.UI.Alert("SUCCESS", "The character creation has been completed.");
                    LoadCharacterInfo();
                    titleUI.OpenPanel("GameStart");
                    break;
                case CreateCharacterResult.DUPLICATE:
                    Managers.UI.Alert("DUPLICATE", "This is a character name that already exists.");
                    break;
                case CreateCharacterResult.ERROR:
                default:
                    Managers.UI.Alert("ERROR", "Error occurred. Error code 101");
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
                Username = Managers.Game.playerData.AccountID
            };

            NetworkClient.ReplaceHandler<CharacterInfoLoadResponseMessage>(OnLoadCharacterInfoResultReceived);
            NetworkClient.Send(loadCharacterRequestMessage);
        }

        private void OnLoadCharacterInfoResultReceived(CharacterInfoLoadResponseMessage msg)
        {
            var firstChapter = titleUI.chapterManager.chapters[0];

            if (msg.CharacterData == null || msg.CharacterData.Count == 0)
            {
                titleUI.chapterManager.chapters.Clear();
                titleUI.chapterManager.chapters.Add(firstChapter);
                titleUI.chapterManager.currentChapterIndex = 0;
                titleUI.chapterManager.InitializeChapters();
                return;
            }

            //데이터 가공
            foreach (var character in msg.CharacterData)
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
                        //캐릭터를 정말 삭제할건지 묻는 팝업을 띄우고 해당 팝업의 Confirm에 삭제 리스너
                        //삭제가 완료된 뒤 창을 닫고, 리스너를 제거함
                        _view.DeleteCharacterPopup.onConfirm.AddListener(() =>
                        {
                            DeleteCharacter(character.title);
                            _view.DeleteCharacterPopup.CloseWindow();
                            _view.DeleteCharacterPopup.onConfirm.RemoveAllListeners();
                        });
                        _view.DeleteCharacterPopup.OpenWindow();
                    });
                }

            }

            // 첫 번째 요소(캐릭터 생성창)를 제외하고 리스트 초기화 후 받아온 정보 추가함.
            if (titleUI.chapterManager.chapters.Count > 1)
            {
                titleUI.chapterManager.chapters.Clear();
                titleUI.chapterManager.chapters.Add(firstChapter);
            }

            titleUI.chapterManager.chapters.AddRange(msg.CharacterData);
            titleUI.chapterManager.InitializeChapters();
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
                Username = characterName
            };

            NetworkClient.ReplaceHandler<CharacterDataResponseMessage>(OnCharacterDataReceived);
            NetworkClient.Send(characterDataRequestMessage);
        }

        private void OnCharacterDataReceived(CharacterDataResponseMessage msg)
        {
            //캐릭터의 정보를 처음 받아올 때는 서버로 다시 전송할 필요 없음
            Managers.Game.suppressDataChangeEvents = false;

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

            //캐릭터 정보 로드가 모두 완료된 이후에는 값의 변화가 생기면 다시 서버로 전송필요
            Managers.Game.suppressDataChangeEvents = true;

            DebugUtils.Log($"Player data loaded: {Managers.Game.playerData.Username}");
            
            //캐릭터 정보를 모두 받아왔으면 인게임으로 씬 전환을 시작
            
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
                Username = characterName
            };

            NetworkClient.ReplaceHandler<DeleteCharacterResponseMessage>(OnDeleteCharacterReceived);
            NetworkClient.Send(deleteCharacterRequestMessage);
        }

        private void OnDeleteCharacterReceived(DeleteCharacterResponseMessage msg)
        {
            if (true == msg.Result)
            {
                LoadCharacterInfo();
            }
        }
    }
}