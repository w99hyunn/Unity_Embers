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
            if (other.TryGetComponent<NPCInteract>(out NPCInteract npc))
            {
                currentNPC = npc;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<NPCInteract>(out NPCInteract npc) && npc == currentNPC)
            {
                currentNPC = null;
            }
        }

        /// <summary>
        /// F키를 눌러 대화 상호작용
        /// </summary>
        private void OnInteract()
        {
            if (currentNPC != null
                && currentNPC.CanTalk()
                && playerController.State == PlayerController.PlayerState.Normal)
            {
                playerController.State = PlayerController.PlayerState.NpcTalk;
                dialogueManager.StartDialogue(currentNPC, EndDialogueCallback);
            }
        }

        private void EndDialogueCallback()
        {
            playerController.State = PlayerController.PlayerState.Normal;
            currentNPC.EndDialogue();
        }
    }
}
