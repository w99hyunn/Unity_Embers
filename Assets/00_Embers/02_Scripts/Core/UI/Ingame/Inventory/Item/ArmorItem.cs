namespace Embers
{
    /// <summary> 장비 - 방어구 아이템 </summary>
    public class ArmorItem : EquipmentItem
    {
        public ArmorItemData ArmorData => EquipmentData as ArmorItemData;

        public ArmorItem(ArmorItemData data) : base(data) { }
    }
}
