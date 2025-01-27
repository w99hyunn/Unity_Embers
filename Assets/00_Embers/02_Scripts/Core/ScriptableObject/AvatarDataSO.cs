using System.Collections.Generic;
using UnityEngine;

namespace NOLDA
{
    [System.Serializable]
    public class AvatarData
    {
        public Class classType;
        public GameObject avatarPrefab;
    }
    
    [CreateAssetMenu(fileName = "AvatarData", menuName = "NOLDA/AvatarData", order = 1)]
    public class AvatarDataSO : ScriptableObject
    {
        public List<AvatarData> avatarList;
    }
}