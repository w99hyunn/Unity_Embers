using UnityEngine;

namespace NOLDA
{
    public class PlayerNPCDialogueHandler : MonoBehaviour
    {
        private PlayerController playerController;
        private DialogueManager dialogueManager;
        private NPCInteract currentNPC;

        private void Awake()
        {
            TryGetComponent<PlayerController>(out playerController);
            dialogueManager = FindAnyObjectByType<DialogueManager>();
        }

        private void OnTriggerEnter(Collider other)
        {
            NPCInteract npc = other.GetComponent<NPCInteract>();
            if (npc != null)
            {
                currentNPC = npc;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<NPCInteract>() == currentNPC)
            {
                currentNPC = null;
            }
        }

        private void OnInteract()
        {
            if (currentNPC != null && currentNPC.CanTalk())
            {
                dialogueManager.StartDialogue(currentNPC, EndDialogueCallback);
                playerController.isNpcTalk = true;
            }
        }

        private void EndDialogueCallback()
        {
            playerController.isNpcTalk = false;
        }
    }
}
