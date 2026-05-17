using UnityEngine;

namespace Embers
{
    /// <summary> 수량 아이템 - 포션 아이템 </summary>
    public class PortionItem : CountableItem, IUsableItem
    {
        public PortionItem(PortionItemData data, int amount = 1) : base(data, amount) { }

        /// <summary>
        /// 포션 아이템 사용시 일어날 행동 현재는 1개 감소
        /// </summary>
        /// <returns></returns>
        public bool Use()
        {
            PortionItemData portionData = CountableData as PortionItemData;
            int effectValue = Mathf.RoundToInt(portionData.Value);

            switch (portionData.EffectType)
            {
                case PortionEffectType.HP:
                    Singleton.Game.playerData.Hp += effectValue;
                    break;
                case PortionEffectType.MP:
                    Singleton.Game.playerData.Mp += effectValue;
                    break;
            }

            Amount--;

            return true;
        }

        protected override CountableItem Clone(int amount)
        {
            return new PortionItem(CountableData as PortionItemData, amount);
        }
    }
}
