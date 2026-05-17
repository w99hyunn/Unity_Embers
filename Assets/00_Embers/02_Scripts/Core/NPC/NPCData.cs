using UnityEngine;

namespace Embers
{
    [CreateAssetMenu(fileName = "NPC_0", menuName = "Embers/NPC/NPC Data")]
    public class NPCData : ScriptableObject
    {
        public string npcName;
        public string npcRole;
        [TextArea(3, 5)] public string[] dialogueLines;
    }
}