using UnityEngine;

namespace NOLDA
{
    public class PlayerNPCDialogueHandler : MonoBehaviour
    {
        DialogueManager dialogueManager;
        NPCInteract currentNPC;

        void Awake()
        {
            dialogueManager = FindAnyObjectByType<DialogueManager>();
        }

        void OnTriggerEnter(Collider other)
        {
            NPCInteract npc = other.GetComponent<NPCInteract>();
            if (npc != null)
            {
                currentNPC = npc;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<NPCInteract>() == currentNPC)
            {
                currentNPC = null;
            }
        }

        void OnInteract()
        {
            if (currentNPC != null && currentNPC.CanTalk())
            {
                Debug.Log("까꿍");
                //dialogueManager.StartDialogue(currentNPC);
            }
        }
    }
}
