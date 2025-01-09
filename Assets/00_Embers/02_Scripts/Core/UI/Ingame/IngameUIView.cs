using System;
using Michsky.UI.Reach;
using UnityEngine;

namespace STARTING
{
    public class IngameUIView : MonoBehaviour
    {
        [Header("Fade In")]
        public ImageFading initPanel;
        
        [Header("Player HUD")]
        [Header("Stats")]
        public ProgressBar playerHP;
        public ProgressBar playerMP;
        public ProgressBar playerHxp;

        private void Awake()
        {
            if (initPanel != null) { initPanel.gameObject.SetActive(true); }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
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
            if (fieldName == "Hp")
            {
                playerHP.SetValue(Convert.ToInt32(newValue));
            }
            else if (fieldName == "Mp")
            {
                playerMP.SetValue(Convert.ToInt32(newValue));
            }
            else if (fieldName == "Exp")
            {
                playerHxp.SetValue(Convert.ToInt32(newValue));
            }
        }
        
        async Awaitable StartInitialize()
        {
            await Awaitable.WaitForSecondsAsync(0.5f);
            if (initPanel != null) { initPanel.FadeOut(); }
        }
        
        private void StartHUDInit()
        {
            //HP, MP Init
            playerHP.maxValue = Managers.Game.playerData.MaxHp;
            playerMP.maxValue = Managers.Game.playerData.MaxMp;
            playerHxp.maxValue = Managers.Game.playerData.Hxp;
            playerHP.SetValue(Managers.Game.playerData.Hp);
            playerMP.SetValue(Managers.Game.playerData.Mp);
            playerHxp.SetValue(Managers.Game.playerData.Hxp);
        }
        
        // Update is called once per frame
        void Update()
        {

        }
    }
}