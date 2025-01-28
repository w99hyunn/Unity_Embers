using UnityEngine;
using UnityEngine.EventSystems;

namespace NOLDA
{
    public class MovableHeaderUI : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        private Transform targetTransform;

        private Vector2 _beginPoint;
        private Vector2 _moveBegin;

        private void Awake()
        {
            if(targetTransform == null)
                targetTransform = transform.parent;
        }
        
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            _beginPoint = targetTransform.position;
            _moveBegin = eventData.position;
        }
        
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            targetTransform.position = _beginPoint + (eventData.position - _moveBegin);
        }
    }
}