using UnityEngine;
using UnityEngine.EventSystems;

namespace NOLDA
{
    /// <summary>
    /// UI 헤더에 부착. 움직이는 기능은 제거하고 눌렀을 때 최상위로 오는 기능만 존재
    /// </summary>
    public class StaticHeaderUI : MonoBehaviour, IPointerDownHandler
    {
        private Transform targetTransform;

        private void Awake()
        {
            if (targetTransform == null)
                targetTransform = transform.parent;
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            targetTransform.SetAsLastSibling();
        }
    }
}