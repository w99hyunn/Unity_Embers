using UnityEngine;

namespace NOLDA
{
    public class CharacterInfoUIController : MonoBehaviour
    {
        public CharacterInfoUIView _view;

        private void Start()
        {
            _view.nameText.text = Singleton.Game.playerData.Username;
            _view.levelText.text = "Lv. " + Singleton.Game.playerData.Level.ToString();
            _view.classText.text = Singleton.Game.playerData.Class.ToString();
            _view.factionText.text = Singleton.Game.playerData.Faction.ToString();
            _view.hpText.text = Singleton.Game.playerData.Hp.ToString();
            _view.mpText.text = Singleton.Game.playerData.Mp.ToString();
            _view.attackText.text = Singleton.Game.playerData.Attack.ToString();
            _view.armorText.text = Singleton.Game.playerData.Armor.ToString();
        }

        public void OnEnable()
        {
            Singleton.Game.playerData.OnDataChanged += HandleDataChanged;
        }

        private void OnDisable()
        {
            Singleton.Game.playerData.OnDataChanged -= HandleDataChanged;
        }

        private void HandleDataChanged(string fieldName, object newValue)
        {
            switch (fieldName)
            {
                case nameof(Singleton.Game.playerData.Faction):
                    _view.factionText.text = newValue.ToString();
                    break;
                case nameof(Singleton.Game.playerData.Class):
                    _view.classText.text = newValue.ToString();
                    break;
                case nameof(Singleton.Game.playerData.Hp):
                    _view.hpText.text = newValue.ToString();
                    break;
                case nameof(Singleton.Game.playerData.Mp):
                    _view.mpText.text = newValue.ToString();
                    break;
                case nameof(Singleton.Game.playerData.Attack):
                    _view.attackText.text = newValue.ToString();
                    break;
                case nameof(Singleton.Game.playerData.Armor):
                    _view.armorText.text = newValue.ToString();
                    break;
                case nameof(Singleton.Game.playerData.Level):
                    _view.levelText.text = "Lv. " + newValue.ToString();
                    break;
            }
        }
    }
}