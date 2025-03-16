using UnityEngine;
using System;
using Michsky.UI.Reach;

namespace NOLDA
{
    public class DialogueUIController : MonoBehaviour
    {
        [SerializeField] private DialogueUIView _view;
        [SerializeField] private UIControlManager uiControlManager;
        [SerializeField] private ModalWindowManager dialogueUI;

        private string[] currentDialogue;
        private int dialogueIndex = 0;
        private bool _isTyping = false;
        private Action onDialogueEnd;

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
            onDialogueEnd?.Invoke();
        }

        public void ShowNextDialogue()
        {
            if (_isTyping) return;
            TypeDialogue(currentDialogue[dialogueIndex]).Forget();
        }

        public void EndDialogue()
        {
            uiControlManager.CloseUI(dialogueUI);
            onDialogueEnd?.Invoke();
        }

        public void StartDialogue(NPCInteract npc, Action onEndCallback)
        {
            _view.SetDefaultNpcInfo(npc.GetNPCData().npcName, npc.GetNPCData().npcRole);
            currentDialogue = npc.GetNPCData().dialogueLines;
            dialogueIndex = 0;
            onDialogueEnd = onEndCallback;
            _view.ShowNextButton(false);
            _view.ShowOkButton(false);

            uiControlManager.OpenUI(dialogueUI);

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
    }
}