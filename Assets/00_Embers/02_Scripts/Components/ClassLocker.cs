using System.Collections.Generic;
using Michsky.UI.Reach;
using UnityEngine;

namespace NOLDA
{
    public class ClassLocker : MonoBehaviour
    {
        [Tooltip("Class Selector의 items에서 잠길 클래스는 index에 맞는 bool 값을 false로 설정해주세요.")]
        [SerializeField] private List<bool> openClasses = new List<bool>();
        private ModeSelector classSelector;

        void Awake()
        {
            TryGetComponent<ModeSelector>(out classSelector);
            UpdateClassSelector();
        }

        private void UpdateClassSelector()
        {
            for (int i = classSelector.items.Count - 1; i >= 0; i--)
            {
                if (openClasses[i] == false)
                {
                    classSelector.items.RemoveAt(i);
                }
            }
        }
    }
}