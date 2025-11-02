using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

namespace NOLDA
{
    public class DialogueUIController : MonoBehaviour
    {
        [SerializeField] private DialogueUIView _view;
        [SerializeField] private UIControlManager uiControlManager;
        private string[] currentDialogue;
        private int dialogueIndex = 0;
        private bool _isTyping = false;
        private Action onDialogueEnd;
        private Dictionary<CanvasGroup, CancellationTokenSource> fadeCancellationTokens = new Dictionary<CanvasGroup, CancellationTokenSource>();

        private void Start()
        {
            _view.NextButtonAction += ShowNextDialogue;
            _view.OkButtonAction += EndDialogue;
            _view.CloseButtonAction += EndDialogue;
        }

        private void OnDestroy()
        {
            _view.NextButtonAction -= ShowNextDialogue;
            _view.OkButtonAction -= EndDialogue;
            _view.CloseButtonAction -= EndDialogue;
        }

        private void OnDisable()
        {
            EndDialogue();
        }

        public void ShowNextDialogue()
        {
            if (_isTyping) return;
            TypeDialogue(currentDialogue[dialogueIndex]).Forget();
        }

        public void EndDialogue()
        {
            ShowUI();
            uiControlManager.CloseUI(_view.DialogueUI);
            uiControlManager.MainCamera.cullingMask |= (1 << 3) | (1 << 8);

            onDialogueEnd?.Invoke();
        }

        public void StartDialogue(NPCInteract npc, Action onEndCallback)
        {
            npc.StartDialogue();

            currentDialogue = npc.GetNPCData().dialogueLines;
            dialogueIndex = 0;
            onDialogueEnd = onEndCallback;

            HideUI();
            _view.SetDefaultNpcInfo(npc.GetNPCData().npcName, npc.GetNPCData().npcRole);
            _view.ShowNextButton(false);
            _view.ShowOkButton(false);

            uiControlManager.OpenUI(_view.DialogueUI);
            uiControlManager.MainCamera.cullingMask &= ~((1 << 3) | (1 << 8));

            TypeDialogue(currentDialogue[dialogueIndex]).Forget();
        }

        private async Awaitable TypeDialogue(string dialogue)
        {
            _isTyping = true;
            _view.DialogueText = "";
            dialogueIndex++;

            foreach (char letter in dialogue.ToCharArray())
            {
                _view.DialogueText += letter;
                await Awaitable.WaitForSecondsAsync(0.05f);
            }

            _isTyping = false;

            if (dialogueIndex < currentDialogue.Length)
            {
                _view.ShowNextButton(true);
                _view.ShowOkButton(false);
            }
            else
            {
                _view.ShowNextButton(false);
                _view.ShowOkButton(true);
            }
        }

        public void HideUI()
        {
            foreach (var canvasGroup in _view.HideUIsOnDialogue)
            {
                CancelExistingFade(canvasGroup);
                FadeCanvasGroup(canvasGroup, false).Forget();
            }
        }

        public void ShowUI()
        {
            foreach (var canvasGroup in _view.HideUIsOnDialogue)
            {
                CancelExistingFade(canvasGroup);
                FadeCanvasGroup(canvasGroup, true).Forget();
            }
        }

        private void CancelExistingFade(CanvasGroup canvasGroup)
        {
            if (fadeCancellationTokens.TryGetValue(canvasGroup, out var tokenSource))
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
                fadeCancellationTokens.Remove(canvasGroup);
            }
        }

        private async Awaitable FadeCanvasGroup(CanvasGroup canvasGroup, bool fadeIn)
        {
            var tokenSource = new CancellationTokenSource();
            fadeCancellationTokens[canvasGroup] = tokenSource;
            var token = tokenSource.Token;

            try
            {
                float duration = 0.5f;
                float elapsedTime = 0f;
                float startAlpha = canvasGroup.alpha;
                float endAlpha = fadeIn ? 1 : 0;

                if (fadeIn)
                {
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }

                while (elapsedTime < duration && !token.IsCancellationRequested)
                {
                    elapsedTime += Time.deltaTime;
                    canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
                    await Awaitable.NextFrameAsync();
                }

                if (!token.IsCancellationRequested)
                {
                    canvasGroup.alpha = endAlpha;

                    if (!fadeIn)
                    {
                        canvasGroup.interactable = false;
                        canvasGroup.blocksRaycasts = false;
                    }
                }
            }
            finally
            {
                if (fadeCancellationTokens.TryGetValue(canvasGroup, out var currentTokenSource) && currentTokenSource == tokenSource)
                {
                    fadeCancellationTokens.Remove(canvasGroup);
                    tokenSource.Dispose();
                }
            }
        }
    }
}