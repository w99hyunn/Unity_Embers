using System.Collections.Generic;
using UnityEngine;

namespace STARTING
{
    [System.Serializable]
    public class ChunkInfo
    {
        [Tooltip("청크 이름(지역 이름)")]
        public string chunkDisplayName;

        [Tooltip("유니티 씬 이름 ('Chunk_0_0')")]
        public string sceneName;
    }
    
    [CreateAssetMenu(fileName = "ChunkList", menuName = "STARTING/ChunkList", order = 1)]
    public class ChunkListSO : ScriptableObject
    {
        public List<ChunkInfo> chunkSceneNames;
    }
}