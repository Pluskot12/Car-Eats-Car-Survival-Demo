using System.Collections.Generic;
using UnityEngine;
using static CarGame.DropTable;

namespace CarGame
{
    public class ChestRandomLoot : MonoBehaviour
    {
        [Header("Drops")]
        [SerializeField] private int minItems = 1;
        [SerializeField] private int maxItems = 3;
        [SerializeField] private DropTable[] dropsTables;

        public void RandomizeContent(InventoryController inventory) 
        {
            int max = Mathf.Min(maxItems, inventory.Capacity);

            List<DroppedItem> drops = new List<DroppedItem>();

            drops.AddRange(Roll(minItems, max));

            for (int i = 0; i < drops.Count; i++) 
            {
                inventory.TryAddItem(drops[i].item, drops[i].quantity);
            }
        }

        private IEnumerable<DroppedItem> Roll(int minItems, int maxItems, float multiplier = 1f)
        {
            List<DropTable.Item> possibleItems = new List<DropTable.Item>();

            foreach (var table in dropsTables) 
            {
                possibleItems.AddRange(table.items);
            }

            List<DropTable.Item> shuffledItems = new List<DropTable.Item>(possibleItems);
            for (int i = shuffledItems.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                var temp = shuffledItems[i];
                shuffledItems[i] = shuffledItems[j];
                shuffledItems[j] = temp;
            }

            List<DroppedItem> drops = new List<DroppedItem>();
            HashSet<ItemData> droppedItemTypes = new HashSet<ItemData>();

            foreach (DropTable.Item item in shuffledItems)
            {
                if (drops.Count >= maxItems) break;

                float random = Random.Range(0f, 100f);
                if (random <= item.dropChance * multiplier)
                {
                    DroppedItem droppedItem = new DroppedItem();
                    droppedItem.item = item.item;
                    droppedItem.quantity = Random.Range(item.minDrop, item.maxDrop);
                    drops.Add(droppedItem);
                    droppedItemTypes.Add(item.item);
                }
            }

            while (drops.Count < minItems && droppedItemTypes.Count < shuffledItems.Count)
            {
                foreach (DropTable.Item item in shuffledItems)
                {
                    if (drops.Count >= minItems)
                    {
                        break;
                    }

                    if (droppedItemTypes.Contains(item.item))
                    {
                        continue;
                    }

                    float random = Random.Range(0f, 100f);
                    if (random <= item.dropChance * multiplier)
                    {
                        DroppedItem droppedItem = new DroppedItem();
                        droppedItem.item = item.item;
                        droppedItem.quantity = Random.Range(item.minDrop, item.maxDrop);
                        drops.Add(droppedItem);
                        droppedItemTypes.Add(item.item);
                    }
                }
            }

            return drops;
        }
    }
}
