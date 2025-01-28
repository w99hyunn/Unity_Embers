using UnityEngine;
using UnityEngine.EventSystems;

namespace NOLDA
{
    public class MovableHeaderUI : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        public string positionKey;

        private Transform targetTransform;
        private Vector2 _beginPoint;
        private Vector2 _moveBegin;

        private void Awake()
        {
            if(targetTransform == null)
                targetTransform = transform.parent;

            LoadPosition();
        }
        
        private void LoadPosition()
        {
            if (PlayerPrefs.HasKey(positionKey + "_x"))
            {
                float x = PlayerPrefs.GetFloat(positionKey + "_x");
                float y = PlayerPrefs.GetFloat(positionKey + "_y");
                float z = PlayerPrefs.GetFloat(positionKey + "_z");
                targetTransform.position = new Vector3(x, y, z);
            }
            else
            {
                SavePosition();
            }
        }

        private void SavePosition()
        {
            PlayerPrefs.SetFloat(positionKey + "_x", targetTransform.position.x);
            PlayerPrefs.SetFloat(positionKey + "_y", targetTransform.position.y);
            PlayerPrefs.SetFloat(positionKey + "_z", targetTransform.position.z);
            PlayerPrefs.Save();
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            _beginPoint = targetTransform.position;
            _moveBegin = eventData.position;
        }
        
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            targetTransform.position = _beginPoint + (eventData.position - _moveBegin);
            SavePosition();
        }
    }
}