using System;
using System.Collections.Generic;
using UnityEngine;

namespace CarGame
{
    public class InventoryController : MonoBehaviour
    {
        [System.Serializable]
        public class StarterItems 
        {
            public ItemData item;
            public int quantity = 1;
        }

        public event Action<InventoryItem[]> AnyValueChanged = delegate { };

        [SerializeField] private InventoryUI inventoryUI;
        [SerializeField] private int capacity = 21;
        [SerializeField] private List<StarterItems> testItems;

        private Inventory inventory;
        public Inventory Inventory => inventory;

        public int Capacity => capacity;
        public int ItemCount => inventory.ItemsCount();

        private void Awake()
        {
            inventory = new Inventory(capacity);

            inventory.AnyValueChanged += OnInventoryChanged;
            inventoryUI.AnyValueChanged += OnDragEnded;
            inventoryUI.ItemDropped += OnItemDropped;
            inventoryUI.ItemQuantityChanged += OnItemQuantityChanged;
            inventoryUI.OnItemChanged += OnItemChanged;

            inventory.OnItemDestroyed += OnItemDestroyed;

            foreach (var item in testItems) 
            {
                inventory.TryAdd(item.item, item.quantity, ItemSpawner.GetMaxDurability(item.item));
            }

        }

        private void OnItemDestroyed()
        {
            inventoryUI.OnItemBreak();
        }

        private void OnItemDropped(InventorySlotUI uI, Vector3 vector)
        {
            inventory.TryRemoveAtIndex(uI.Index);
        }

        private void OnItemChanged(InventorySlotUI uI)
        {
            inventory.TryRemoveAtIndex(uI.Index);
        }

        private void OnItemQuantityChanged(InventorySlotUI uI, int arg2)
        {
            inventory.TryRemoveQuantityAtIndex(uI.Index, arg2);
        }

        public void OnDragEnded(InventorySlotUI from, InventorySlotUI to)
        {
            if (to.SlottedItem == null)
            {
                inventory.TryAddAtIndex(to.Index, from.SlottedItem.ItemData, from.SlottedItem.Quantity, from.SlottedItem.Durability);
                from.Setup(null);
                UIMananger.IsHoldingItem = false;

                return;
            }

            var leftover = inventory.TryAddAtIndex(to.Index, from.SlottedItem.ItemData, from.SlottedItem.Quantity, from.SlottedItem.Durability);

            if (leftover != null && leftover.Quantity > 0 || leftover.ItemData.GetType() == typeof(WeaponItemData))
            {
                from.Setup(leftover);
            }
            else
            {
                from.Setup(null);
            }
        }

        public void OnQuantityChanged(InventorySlotUI from, int quantity) 
        {
            inventory.TryRemoveQuantityAtIndex(from.Index, quantity);
        }

        public void OnInventoryChanged(InventoryItem[] items) 
        {
            AnyValueChanged.Invoke(items);

            inventoryUI.Refresh(items);
        }

        public int OnItemPickup(ItemData item, int quantity, int durability)
        {
            int remaining = inventory.TryAdd(item, quantity, durability);
            inventoryUI.OnItemPickup();
            return remaining;
        }

        public bool CanFit(ItemData data, int quantity)
        {
            return inventory.CanFit(data, quantity);
        }

        public void OnItemUse(int slot) 
        {
            inventory.TryRemoveQuantityAtIndex(slot, 1);
            inventory.TryDamageItemAtIndex(slot, 1);
        }

        public void DamageItem(int slot)
        {
            inventory.TryDamageItemAtIndex(slot, 1);
        }



        public int GetCountAtIndex(int slot) 
        {
            if (inventory.Items[slot] != null) 
            {
                return inventory.Items[slot].Quantity;
            }

            return -1;
            
        }

        public int GetCountOfType(ItemData item) => inventory.GetItemCount(item);

        public int RemoveItems(ItemData item, int amount) 
        {
            return inventory.RemoveItems(item, amount);
        }

        public void TryAddItem(ItemData item, int amount) 
        {
            int durability = -1;
            inventory.TryAdd(item, amount, durability);
        }

        public void AddAtIndex(int index, ItemData data, int q, int d) 
        {
            inventory.TryAddAtIndex(index, data, q, d);
        }

        public void DropAllItems()
        {
            Vector3 force = Vector3.up;
            Vector3 position = GameManager.Instance.Player.transform.position;
            
            foreach (var item in inventory.Items) 
            {
                if (item == null) 
                {
                    continue;
                }

                ItemSpawner.Instance.DropItemOnDeath(item.ItemData, item.Quantity, item.Durability, position, true);
            }

            if (UIMananger.HeldItem != null) 
            {
                ItemSpawner.Instance.DropItemOnDeath(UIMananger.HeldItem.ItemData, UIMananger.HeldItem.Quantity, UIMananger.HeldItem.Durability, position, true);
                UIMananger.Instance.OnPlayerDeath();
            }

            inventory.Clear();
        }
    } 
}