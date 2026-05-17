using System.Collections.Generic;
using Michsky.UI.Reach;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Embers
{
    public class EquipUIController : MonoBehaviour
    {
        private EquipUIView view;
        private InventoryUIController _inventory;
        private ModalWindowManager modalWindow;
        private PointerEventData pointerEventData;
        private readonly List<RaycastResult> raycastResults = new List<RaycastResult>(8);

        private void Awake()
        {
            TryGetComponent<EquipUIView>(out view);
            TryGetComponent<ModalWindowManager>(out modalWindow);
            pointerEventData = new PointerEventData(EventSystem.current);
        }

        private void Start()
        {
            modalWindow.onOpen.AddListener(Refresh);
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(1))
            {
                return;
            }

            ItemSlotUI slot = view.RaycastEquipSlot(pointerEventData, raycastResults);
            if (slot == view.WeaponSlot)
            {
                UnequipWeapon();
            }
            else if (slot == view.ArmorSlot)
            {
                UnequipArmor();
            }
        }

        private void OnDestroy()
        {
            modalWindow.onOpen.RemoveListener(Refresh);
        }

        public void Initialize(InventoryUIController inventoryController)
        {
            _inventory = inventoryController;
            Refresh();
        }

        public void Refresh()
        {
            view.SetWeapon(Singleton.Game.playerData.EquippedWeapon);
            view.SetArmor(Singleton.Game.playerData.EquippedArmor);
        }

        public void UnequipWeapon()
        {
            int weaponPosition = Singleton.Game.playerData.EquippedWeaponPosition;

            Singleton.Game.playerData.UnequipEquipment(Singleton.Game.playerData.EquippedWeapon);
            _inventory.UpdateEquippedSlot(weaponPosition);
            Refresh();
        }

        public void UnequipArmor()
        {
            int armorPosition = Singleton.Game.playerData.EquippedArmorPosition;

            Singleton.Game.playerData.UnequipEquipment(Singleton.Game.playerData.EquippedArmor);
            _inventory.UpdateEquippedSlot(armorPosition);
            Refresh();
        }
    }
}
