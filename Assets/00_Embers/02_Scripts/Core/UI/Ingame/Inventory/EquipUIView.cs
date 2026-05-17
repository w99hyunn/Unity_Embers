using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Embers
{
    public class EquipUIView : MonoBehaviour
    {
        [SerializeField] private ItemSlotUI _weaponSlot;
        [SerializeField] private ItemSlotUI _armorSlot;

        public ItemSlotUI WeaponSlot => _weaponSlot;
        public ItemSlotUI ArmorSlot => _armorSlot;

        public void SetWeapon(WeaponItem weaponItem)
        {
            _weaponSlot.SetItem(weaponItem?.Data.IconSprite);
            _weaponSlot.SetItemAmount(1);
        }

        public void SetArmor(ArmorItem armorItem)
        {
            _armorSlot.SetItem(armorItem?.Data.IconSprite);
            _armorSlot.SetItemAmount(1);
        }

        public ItemSlotUI RaycastEquipSlot(PointerEventData pointerEventData, List<RaycastResult> raycastResults)
        {
            pointerEventData.position = Input.mousePosition;
            raycastResults.Clear();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            foreach (RaycastResult result in raycastResults)
            {
                Transform current = result.gameObject.transform;
                while (current != null)
                {
                    if (current.TryGetComponent(out ItemSlotUI slot))
                    {
                        if (slot == _weaponSlot || slot == _armorSlot)
                        {
                            return slot;
                        }
                    }

                    current = current.parent;
                }
            }

            return null;
        }
    }
}
