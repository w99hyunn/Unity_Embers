namespace Embers
{
    /// <summary> 장비 - 무기 아이템 </summary>
    public class WeaponItem : EquipmentItem
    {
        public WeaponItemData WeaponData => EquipmentData as WeaponItemData;

        public WeaponItem(WeaponItemData data) : base(data) { }
    }
}
