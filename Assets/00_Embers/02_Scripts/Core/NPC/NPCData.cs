using UnityEngine;

namespace NOLDA
{
    [CreateAssetMenu(fileName = "NPC_0", menuName = "NOLDA/NPC/NPC Data")]
    public class NPCData : ScriptableObject
    {
        public string npcName;
        public string npcRole;
        [TextArea(3, 5)] public string[] dialogueLines;
    }
}