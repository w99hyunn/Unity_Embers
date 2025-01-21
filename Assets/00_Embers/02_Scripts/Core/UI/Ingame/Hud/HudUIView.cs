using System;
using Michsky.UI.Reach;
using TMPro;
using UnityEngine;

namespace STARTING
{
    public class HudUIView : MonoBehaviour
    {
        [Header("Player HUD")]
        [Header("Stats")]
        public TMP_Text playerLevel;
        public ProgressBar playerHp;
        public ProgressBar playerMp;
        public ProgressBar playerHxp;

        [Header("Minimap")]
        public ButtonManager mapName;
        
        void Start()
        {
            StartHUDInit();
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
                case "Level":
                    playerLevel.text = Convert.ToString(newValue);
                    MaxHxpSet();
                    break;
                case "Hxp":
                    playerHxp.SetValue(Convert.ToInt32(newValue));
                    break;
                case "MaxHp":
                    playerHp.SetMaxValue(Convert.ToInt32(newValue));
                    break;
                case "MaxMp":
                    playerMp.SetMaxValue(Convert.ToInt32(newValue));
                    break;
                case "Hp":
                    playerHp.SetValue(Convert.ToInt32(newValue));
                    break;
                case "Mp":
                    playerMp.SetValue(Convert.ToInt32(newValue));
                    break;
            }
        }
        
        private void StartHUDInit()
        {
            //Level Init
            playerLevel.text = Singleton.Game.playerData.Level.ToString();
            playerHxp.SetValue(Singleton.Game.playerData.Hxp);
            MaxHxpSet();
            
            //HP, MP Init
            playerHp.SetMaxValue(Singleton.Game.playerData.MaxHp);
            playerMp.SetMaxValue(Singleton.Game.playerData.MaxMp);
            playerHp.SetValue(Singleton.Game.playerData.Hp);
            playerMp.SetValue(Singleton.Game.playerData.Mp);
        }

        private void MaxHxpSet()
        {
            playerHxp.SetMaxValue(
                Singleton.Game.playerData.hxpTable.GetExperienceForNextLevel((Singleton.Game.playerData.Level)));
        }
    }
}