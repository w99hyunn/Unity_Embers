using UnityEngine;

namespace NOLDA
{
    /// <summary> 셀 수 있는 (중첩) 아이템 ex. 소비템</summary>
    public abstract class CountableItemData : ItemData
    {
        public int MaxAmount => _maxAmount;
        [SerializeField] private int _maxAmount = 99;
    }
}