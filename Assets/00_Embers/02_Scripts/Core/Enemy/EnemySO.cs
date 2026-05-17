using System;
using System.Collections.Generic;
using UnityEngine;

namespace Embers
{
    [CreateAssetMenu(fileName = "Enemy_", menuName = "Embers/Enemy/Enemy Data", order = 1)]
    public class EnemySO : ScriptableObject
    {
        public string EnemyName => enemyName;
        public float MaxHp => maxHp;
        public int Hxp => hxp;
        public IReadOnlyList<DropEntry> DropEntries => dropEntries;

        [SerializeField] private string enemyName;
        [SerializeField] private float maxHp = 100f;
        [SerializeField] private int hxp;
        [SerializeField] private List<DropEntry> dropEntries = new List<DropEntry>();

        [Serializable]
        public class DropEntry
        {
            public ItemData ItemData => itemData;
            public float DropChance => dropChance;
            public int Amount => Mathf.Max(1, amount);

            [SerializeField] private ItemData itemData;
            [Range(0f, 1f)][SerializeField] private float dropChance = 1f;
            [SerializeField] private int amount = 1;
        }
    }
}
