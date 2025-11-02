using TMPro;
using UnityEngine;


namespace NOLDA
{
    public class NPCNameTag : MonoBehaviour
    {
        [SerializeField] private TMP_Text npcRole;
        [SerializeField] private TMP_Text npcName;
        [SerializeField] private GameObject interactUI;

        public void SetNPCInfo(string npcRole, string npcName)
        {
            this.npcRole.text = npcRole;
            this.npcName.text = npcName;
        }

        public void SetInteractUI(bool index)
        {
            interactUI.SetActive(index);
        }
    }
}