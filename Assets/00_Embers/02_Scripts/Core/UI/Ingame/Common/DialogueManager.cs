using System;
using UnityEngine;

namespace Embers
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private DialogueUIController dialogueUIController;

        public void StartDialogue(NPCInteract npc, Action onEndCallback)
        {

            dialogueUIController.StartDialogue(npc, onEndCallback);
        }
    }
}