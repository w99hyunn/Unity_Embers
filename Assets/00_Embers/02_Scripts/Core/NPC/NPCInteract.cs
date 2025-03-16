using Mirror;
using UnityEngine;

namespace NOLDA
{
    /// <summary>
    /// NPC에 부착
    /// 가까이 다가온 오브젝트가 로컬 플레이어인지 확인하고, npcData 전달 준비
    /// </summary>
    public class NPCInteract : MonoBehaviour
    {
        [SerializeField] private NPCNameTag npcNameTag;
        [SerializeField] private NPCData npcData;

        private bool isPlayerInRange = false;

        void Awake()
        {
            UpdateNPCInfo();
            npcNameTag.SetInteractUI(false);
        }

        void UpdateNPCInfo()
        {
            npcNameTag.SetNPCInfo(npcData.npcRole, npcData.npcName);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == NetworkClient.localPlayer.gameObject)
            {
                isPlayerInRange = true;
                npcNameTag.SetInteractUI(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == NetworkClient.localPlayer.gameObject)
            {
                isPlayerInRange = false;
                npcNameTag.SetInteractUI(false);
            }
        }

        public bool CanTalk() => isPlayerInRange;
        public NPCData GetNPCData() => npcData;
    }
}