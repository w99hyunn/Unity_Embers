using System;
using Michsky.UI.Reach;
using TMPro;
using UnityEngine;

namespace STARTING
{
    public class IngameUIView : MonoBehaviour
    {
        [Header("Fade In")]
        public ImageFading initPanel;

        [Header("Player HUD")] [Header("Stats")]
        public TMP_Text playerLevel;
        public ProgressBar playerHp;
        public ProgressBar playerMp;
        public ProgressBar playerHxp;

        private void Awake()
        {
            if (initPanel != null) { initPanel.gameObject.SetActive(true); }
        }
        
        void Start()
        {
            _ = StartInitialize();
            StartHUDInit();
        }

        public void OnEnable()
        {
            Managers.Game.playerData.OnDataChanged += HandleDataChanged;
        }

        private void OnDisable()
        {
            Managers.Game.playerData.OnDataChanged -= HandleDataChanged;
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
                    playerHp.maxValue = Convert.ToInt32(newValue);
                    break;
                case "MaxMp":
                    playerMp.maxValue = Convert.ToInt32(newValue);
                    break;
                case "Hp":
                    playerHp.SetValue(Convert.ToInt32(newValue));
                    break;
                case "Mp":
                    playerMp.SetValue(Convert.ToInt32(newValue));
                    break;
            }
        }
        
        async Awaitable StartInitialize()
        {
            await Awaitable.WaitForSecondsAsync(0.5f);
            if (initPanel != null) { initPanel.FadeOut(); }
        }
        
        private void StartHUDInit()
        {
            //Level Init
            playerLevel.text = Managers.Game.playerData.Level.ToString();
            playerHxp.SetValue(Managers.Game.playerData.Hxp);
            MaxHxpSet();
            
            //HP, MP Init
            playerHp.maxValue = Managers.Game.playerData.MaxHp;
            playerMp.maxValue = Managers.Game.playerData.MaxMp;
            playerHp.SetValue(Managers.Game.playerData.Hp);
            playerMp.SetValue(Managers.Game.playerData.Mp);
        }

        private void MaxHxpSet()
        {
            playerHxp.maxValue =
                Managers.Game.playerData.hxpTable.GetExperienceForNextLevel((Managers.Game.playerData.Level));
        }
    }
}