using System.Collections.Generic;
using UnityEngine;

namespace CarGame
{
    [CreateAssetMenu(menuName = "Car/Drop Table")]
    public class DropTable : ScriptableObject
    {
        [System.Serializable]
        public class Item
        {
            public ItemData item;
            [Range(0, 100)] public float dropChance;
            public int minDrop;
            public int maxDrop;
        }

        public struct DroppedItem 
        {
            public ItemData item;
            public int quantity;
        }

        public List<Item> items;

        public IEnumerable<DroppedItem> Roll()
        {
            List<DroppedItem> drops = new List<DroppedItem>();
            DroppedItem droppedItem = new DroppedItem();

            foreach (Item item in items) 
            {
                float random = Random.Range(0f, 100f);
                if (random <= item.dropChance) 
                {
                    droppedItem.item = item.item;
                    droppedItem.quantity = Random.Range(item.minDrop, item.maxDrop);
                    drops.Add(droppedItem);
                }
            }

            return drops;
        }
    }
}
