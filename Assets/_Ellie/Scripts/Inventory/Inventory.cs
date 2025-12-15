using System;
using UnityEngine;
using static UnityEngine.Analytics.IAnalytic;

namespace CarGame
{
    public class InventoryItem 
    {
        public ItemData ItemData;
        public int Quantity;
        public int Durability;

        public InventoryItem(ItemData itemData, int quantity)
        {
            ItemData = itemData;
            Quantity = quantity;

            if (itemData is IBreakable breakable) 
            {
                Durability = breakable.MaxDurability;
            }
        }
    }

    public class Inventory
    {
        private InventoryItem[] items;
        public InventoryItem[] Items => items;

        public event Action<InventoryItem[]> AnyValueChanged = delegate { };
        public event Action OnItemDestroyed = delegate { };

        public Inventory(int slots) 
        {
            items = new InventoryItem[slots];
        }

        public int TryAdd(ItemData item, int quantity)
        {
            int remaining = quantity;

            #region Special Case for Weapons

            if (item.GetType() == typeof(WeaponItemData)) 
            {
                for (int i = 0; i < items.Length && remaining >= 0; i++)
                {
                    if (items[i] != null)
                        continue;

                    items[i] = new InventoryItem(item, quantity);
                    remaining = -1;
                }

                AnyValueChanged?.Invoke(items);
                return remaining;
            }

            #endregion

            if (item.maxStackSize == 1)
            {
                for (int i = 0; i < items.Length && remaining > 0; i++)
                {
                    if (items[i] != null)
                        continue;

                    items[i] = new InventoryItem(item, 1);
                    remaining -= 1;
                }

                AnyValueChanged?.Invoke(items);
                return remaining;
            }

            for (int i = 0; i < items.Length && remaining > 0; i++)
            {
                if (items[i] == null)
                    continue;

                if (items[i].ItemData != item)
                    continue;

                int space = items[i].ItemData.maxStackSize - items[i].Quantity;
                if (space <= 0)
                    continue;

                int addAmount = Mathf.Min(space, remaining);
                items[i].Quantity += addAmount;
                remaining -= addAmount;
            }

            for (int i = 0; i < items.Length && remaining > 0; i++)
            {
                if (items[i] != null)
                    continue;

                int addAmount = Mathf.Min(item.maxStackSize, remaining);
                items[i] = new InventoryItem(item, addAmount);
                remaining -= addAmount;
            }

            AnyValueChanged?.Invoke(items);

            return remaining;
        }

        public InventoryItem TryAddAtIndex(int index, ItemData item, int quantity)
        {
            int remaining = quantity;

            if (items[index] == null)
            {
                int addAmount = Mathf.Min(item.maxStackSize, remaining);
                remaining -= addAmount;
                items[index] = new InventoryItem(item, addAmount);
                AnyValueChanged.Invoke(items);
                return new InventoryItem(item, remaining);
            }
            else if (items[index].ItemData == item)
            {
                int maxStack = item.maxStackSize;
                int total = quantity + items[index].Quantity;

                int newQuantity = Mathf.Min(total, maxStack);
                items[index].Quantity = newQuantity;

                int remainder = total - newQuantity;

                AnyValueChanged.Invoke(items);
                return new InventoryItem(item, remainder);
            }

            else if (items[index].ItemData != item) 
            {
                var temp = items[index];
                items[index] = new InventoryItem(item, remaining);

                AnyValueChanged.Invoke(items);
                return temp;
            }

            return null;
        }

        public bool CanFit(ItemData item, int quantity)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null) continue;
                if (items[i].ItemData != item) continue;
                if (item.maxStackSize == 1) continue;
                if (item.GetType() == typeof(WeaponItemData)) continue;

                int space = item.maxStackSize - items[i].Quantity;
                if (space > 0)
                    return true;
            }

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                    return true;
            }

            return false;
        }

        public bool TryRemove(ItemData item)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null || items[i].ItemData != item)
                    continue;

                items[i] = null;
                AnyValueChanged.Invoke(items);
                return true;
            }

            return false;
        }

        public bool TryRemoveAtIndex(int index)
        {
            if (items[index] != null) 
            { 
                items[index] = null;
                AnyValueChanged.Invoke(items);
                return true;
            }

            return false;
        }

        public bool TryRemoveQuantityAtIndex(int index, int quantityToRemove)
        {
            if (items[index] != null)
            {
                items[index].Quantity -= quantityToRemove;
                if (items[index].Quantity <= 0 && items[index].ItemData.GetType() != typeof(WeaponItemData)) // TODO: Fix this
                {
                    items[index] = null;
                }

                AnyValueChanged.Invoke(items);
                return true;
            }

            return false;
        }


        public void Swap(int index1, int index2) 
        {
            (items[index1], items[index2]) = (items[index2], items[index1]);

            AnyValueChanged.Invoke(items);
        }

        public int Combine(int index1, int index2)
        {
            var total = items[index1].Quantity + items[index2].Quantity;
            items[index2].Quantity = total;
            TryRemoveAtIndex(index1);

            AnyValueChanged.Invoke(items);
            return total;
        }

        public int GetItemCount(ItemData item)
        {
            int total = 0;

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null) 
                    continue;

                if (items[i].ItemData == item)
                    total += items[i].Quantity;
            }

            return total;
        }

        public int RemoveItems(ItemData item, int amount)
        {
            int remaining = amount;

            for (int i = 0; i < items.Length; i++)
            {
                var invItem = items[i];
                if (invItem == null) 
                    continue;
                if (invItem.ItemData != item) 
                    continue;

                if (invItem.Quantity > remaining)
                {
                    invItem.Quantity -= remaining;
                    remaining = 0;
                    break;
                }
                else
                {
                    remaining -= invItem.Quantity;
                    items[i] = null;
                }
            }

            AnyValueChanged.Invoke(items);
            return remaining;
        }

        public void DamageItem(InventoryItem invItem, int amount)
        {
            if (invItem.ItemData is IBreakable breakable)
            {
                invItem.Durability -= amount;
                if (invItem.Durability <= 0)
                {
                    // Item breaks
                }
            }

            AnyValueChanged.Invoke(items);
        }

        public bool TryDamageItemAtIndex(int index, int quantityToRemove)
        {
            if (items[index] != null)
            {
                if (items[index].ItemData is IBreakable breakable)
                {
                    items[index].Durability -= quantityToRemove;

                    if (items[index].Durability <= 0)
                    {
                        if (TryRemoveAtIndex(index)) 
                        {
                            OnItemDestroyed.Invoke();
                        }
                    }
                }

                AnyValueChanged.Invoke(items);
                return true;
            }

            return false;
        }

    }
}