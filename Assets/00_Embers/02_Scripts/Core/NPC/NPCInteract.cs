using Mirror;
using Unity.Cinemachine;
using UnityEngine;

namespace NOLDA
{
    /// <summary>
    /// NPC에 부착
    /// 가까이 다가온 오브젝트가 로컬 플레이어인지 확인하고, npcData 전달 준비
    /// </summary>
    public class NPCInteract : MonoBehaviour
    {
        [Header("From Resources")]
        [SerializeField] private NPCData npcData;
        [Space(10)]
        [SerializeField] private NPCNameTag npcNameTag;
        [SerializeField] private CinemachineCamera npcCamera;
        private bool isPlayerInRange = false;

        private void Awake()
        {
            UpdateNPCInfo();
            npcNameTag.SetInteractUI(false);
            npcCamera.enabled = false;
        }

        private void UpdateNPCInfo()
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

        public void StartDialogue()
        {
            npcCamera.enabled = true;
        }

        public void EndDialogue()
        {
            npcCamera.enabled = false;
        }

        public bool CanTalk() => isPlayerInRange;
        public NPCData GetNPCData() => npcData;
    }
}