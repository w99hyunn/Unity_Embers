using System.Threading;
using Michsky.UI.Reach;
using TMPro;
using UnityEngine;

namespace Embers
{
    public class EnemyUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup enemyUI;
        [SerializeField] private ProgressBar hp;
        [SerializeField] private TMP_Text enemyName;

        private Enemy parent;
        private float fadeInDuration = 0.3f;
        private float fadeOutDuration = 0.3f;
        private float fadeOutDelay = 5f;
        private CancellationTokenSource fadeOutCancellationTokenSource;

        void Awake()
        {
            TryGetComponent<Enemy>(out parent);
        }


        void OnEnable()
        {
            parent.OnAttacked += OnAttacked;
            Init();
        }

        void OnDisable()
        {
            parent.OnAttacked -= OnAttacked;
            CancelFadeOut();
        }

        private void Init()
        {
            enemyUI.alpha = 0;
            enemyName.text = parent.EnemyName;
            hp.SetMaxValue(parent.MaxHp);
            hp.SetValue(parent.MaxHp);
        }

        private void OnAttacked(float currentHp)
        {
            hp.SetValue(currentHp);
            CancelFadeOut();
            FadeIn();
            FadeOutAfterDelay();
        }

        private async void FadeIn()
        {
            float elapsed = 0f;
            float startAlpha = enemyUI.alpha;

            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                enemyUI.alpha = Mathf.Lerp(startAlpha, 1f, elapsed / fadeInDuration);
                await Awaitable.NextFrameAsync();
            }

            enemyUI.alpha = 1f;
        }

        private async void FadeOutAfterDelay()
        {
            fadeOutCancellationTokenSource = new CancellationTokenSource();
            var token = fadeOutCancellationTokenSource.Token;

            try
            {
                await Awaitable.WaitForSecondsAsync(fadeOutDelay);
                if (!token.IsCancellationRequested)
                {
                    await FadeOut();
                }
            }
            catch { }
        }

        private async Awaitable FadeOut()
        {
            float elapsed = 0f;
            float startAlpha = enemyUI.alpha;

            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                enemyUI.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeOutDuration);
                await Awaitable.NextFrameAsync();
            }

            enemyUI.alpha = 0f;
        }

        private void CancelFadeOut()
        {
            if (fadeOutCancellationTokenSource != null)
            {
                fadeOutCancellationTokenSource.Cancel();
                fadeOutCancellationTokenSource.Dispose();
                fadeOutCancellationTokenSource = null;
            }
        }
    }
}