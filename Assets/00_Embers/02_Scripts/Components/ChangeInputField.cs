using Michsky.UI.Reach;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace STARTING
{
    public class ChangeInputField : MonoBehaviour
    {
        EventSystem system;
        public Selectable firstInput;


        void Start()
        {
            system = EventSystem.current;
            firstInput.Select();
        }

        void OnNextInputField(InputValue input)
        {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
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