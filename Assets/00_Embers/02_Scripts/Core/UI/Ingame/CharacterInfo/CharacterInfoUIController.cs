using UnityEngine;

namespace NOLDA
{
    public class CharacterInfoUIController : MonoBehaviour
    {
        public CharacterInfoUIView _view;

        private void Start()
        {
            _view.nameText.text = Director.Game.playerData.Username;
            _view.levelText.text = "Lv. " + Director.Game.playerData.Level.ToString();
            _view.classText.text = Director.Game.playerData.Class.ToString();
            _view.factionText.text = Director.Game.playerData.Faction.ToString();
            _view.hpText.text = Director.Game.playerData.Hp.ToString();
            _view.mpText.text = Director.Game.playerData.Mp.ToString();
            _view.attackText.text = Director.Game.playerData.Attack.ToString();
            _view.armorText.text = Director.Game.playerData.Armor.ToString();
        }

        public void OnEnable()
        {
            Director.Game.playerData.OnDataChanged += HandleDataChanged;
        }

        private void OnDisable()
        {
            Director.Game.playerData.OnDataChanged -= HandleDataChanged;
        }

        private void HandleDataChanged(string fieldName, object newValue)
        {
            switch (fieldName)
            {
                case nameof(Director.Game.playerData.Faction):
                    _view.factionText.text = newValue.ToString();
                    break;
                case nameof(Director.Game.playerData.Class):
                    _view.classText.text = newValue.ToString();
                    break;
                case nameof(Director.Game.playerData.Hp):
                    _view.hpText.text = newValue.ToString();
                    break;
                case nameof(Director.Game.playerData.Mp):
                    _view.mpText.text = newValue.ToString();
                    break;
                case nameof(Director.Game.playerData.Attack):
                    _view.attackText.text = newValue.ToString();
                    break;
                case nameof(Director.Game.playerData.Armor):
                    _view.armorText.text = newValue.ToString();
                    break;
                case nameof(Director.Game.playerData.Level):
                    _view.levelText.text = "Lv. " + newValue.ToString();
                    break;
            }
        }
    }
}