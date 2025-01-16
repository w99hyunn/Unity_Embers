using System.Collections.Generic;
using UnityEngine;

namespace STARTING
{
    [System.Serializable]
    public class AvatarData
    {
        public Class classType;
        public GameObject avatarPrefab;
    }
    
    [CreateAssetMenu(fileName = "AvatarData", menuName = "STARTING/AvatarData", order = 1)]
    public class AvatarDataSO : ScriptableObject
    {
        public List<AvatarData> avatarList;
    }
}