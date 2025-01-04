using UnityEngine;

namespace STARTING
{
    public class TitleCharacterUIModel : MonoBehaviour
    {
        [SerializeField] private int defaultMapCode = 1;

        public int MapCode => defaultMapCode;
    }
}