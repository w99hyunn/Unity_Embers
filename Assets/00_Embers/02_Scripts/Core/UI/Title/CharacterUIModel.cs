using UnityEngine;

namespace STARTING
{
    public class CharacterUIModel : MonoBehaviour
    {
        [SerializeField] private int defaultMapCode = 1;

        public int MapCode => defaultMapCode;
    }
}