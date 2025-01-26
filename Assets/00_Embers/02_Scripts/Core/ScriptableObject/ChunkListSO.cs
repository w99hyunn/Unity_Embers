using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NOLDA
{
    [System.Serializable]
    public class ChunkInfo
    {
        [Tooltip("청크 이름(지역 이름)")]
        public string chunkDisplayName;

        [Tooltip("유니티 씬 이름 ('Chunk_0_0')")]
        public string sceneName;

        [Tooltip("이 청크에서 재생할 BGM")]
        public AudioClip bgm;
        
#if UNITY_EDITOR
        [Tooltip("씬 객체")]
        public SceneAsset sceneAsset;
#endif
    }
    
    [CreateAssetMenu(fileName = "ChunkList", menuName = "NOLDA/ChunkList", order = 1)]
    public class ChunkListSO : ScriptableObject
    {
        public List<ChunkInfo> chunkSceneNames;
        
        public ChunkInfo GetChunkInfo(string sceneName)
        {
            return chunkSceneNames.Find(chunk => chunk.sceneName == sceneName);
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            foreach (var chunk in chunkSceneNames)
            {
                if (chunk.sceneAsset != null)
                {
                    string fullPath = AssetDatabase.GetAssetPath(chunk.sceneAsset);
                    string sceneNameOnly = System.IO.Path.GetFileNameWithoutExtension(fullPath);
                    chunk.sceneName = sceneNameOnly;
                }
            }
        }
#endif
    }
}