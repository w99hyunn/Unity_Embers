using UnityEngine;

namespace NOLDA
{
    /// <summary>
    /// 소비아이템 중 포션아이템의 정의
    /// </summary>
    [CreateAssetMenu(fileName = "Item_Portion_", menuName = "Inventory System/Item Data/Portion", order = 3)]
    public class PortionItemData : CountableItemData
    {
        /// <summary> 효과량(회복량 등) </summary>
        public float Value => _value;
        [SerializeField] private float _value;
        public override Item CreateItem()
        {
            return new PortionItem(this);
        }
    }
}