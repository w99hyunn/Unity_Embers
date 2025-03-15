using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace NOLDA
{
    public class DialogueManager : MonoBehaviour
    {
        public GameObject dialogueUI;
        public TMP_Text npcNameText;
        public TMP_Text dialogueText;
        public Button nextButton;

        private string[] currentDialogue;
        private int dialogueIndex = 0;
        private bool isTyping = false;

        public void StartDialogue(NPCInteract npc)
        {
            dialogueUI.SetActive(true);
            npcNameText.text = npc.GetNPCData().npcName;
            currentDialogue = npc.GetNPCData().dialogueLines;
            dialogueIndex = 0;
            nextButton.gameObject.SetActive(false);
            StartCoroutine(TypeDialogue(currentDialogue[dialogueIndex]));
        }

        private IEnumerator TypeDialogue(string dialogue)
        {
            isTyping = true;
            dialogueText.text = "";

            foreach (char letter in dialogue.ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(0.05f); // 타이핑 속도 조절
            }

            isTyping = false;
            nextButton.gameObject.SetActive(true);
        }

        public void ShowNextDialogue()
        {
            if (isTyping) return;

            dialogueIndex++;

            if (dialogueIndex < currentDialogue.Length)
            {
                nextButton.gameObject.SetActive(false);
                StartCoroutine(TypeDialogue(currentDialogue[dialogueIndex]));
            }
            else
            {
                EndDialogue();
            }
        }

        public void EndDialogue()
        {
            dialogueUI.SetActive(false);
        }
    }
}