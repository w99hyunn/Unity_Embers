using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace STARTING
{
    public class ChangeInputField : MonoBehaviour
    {
        private EventSystem _system;
        public Selectable firstInput;
        
        void Start()
        {
            _system = EventSystem.current;
            firstInput.Select();
        }

        void OnNextInputField(InputValue input)
        {
            Selectable next = _system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null)
            {
                next.Select();
            }
        }

        //void OnPrevInputField(InputValue input)
        //{
        //    Debug.Log("¿Ã¿¸");
        //    Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
        //    if (next != null)
        //    {
        //        next.Select();
        //    }
        //}
    }
}