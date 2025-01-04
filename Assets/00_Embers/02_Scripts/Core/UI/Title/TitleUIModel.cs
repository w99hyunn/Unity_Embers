using UnityEngine;

namespace STARTING
{
    public class TitleUIModel : MonoBehaviour
    {
        [SerializeField] private int defaultMapCode = 1;

        public int MapCode => defaultMapCode;
    }
}