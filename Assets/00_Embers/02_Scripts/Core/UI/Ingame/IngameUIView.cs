using System;
using Michsky.UI.Reach;
using UnityEngine;

namespace STARTING
{
    public class IngameUIView : MonoBehaviour
    {
        public ImageFading initPanel;

        private void Awake()
        {
            if (initPanel != null) { initPanel.gameObject.SetActive(true); }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            StartInitialize();
        }

        async Awaitable StartInitialize()
        {
            await Awaitable.WaitForSecondsAsync(0.5f);
            if (initPanel != null) { initPanel.FadeOut(); }
        }
        
        // Update is called once per frame
        void Update()
        {

        }
    }
}