using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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
                inventory.TryAdd(item.item, item.quantity);
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
                inventory.TryAddAtIndex(to.Index, from.SlottedItem.ItemData, from.SlottedItem.Quantity);
                from.Setup(null);
                UIMananger.IsHoldingItem = false;

                return;
            }

            var leftover = inventory.TryAddAtIndex(to.Index, from.SlottedItem.ItemData, from.SlottedItem.Quantity);

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

        public int OnItemPickup(ItemData item, int quantity)
        {
            int remaining = inventory.TryAdd(item, quantity);
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

        public void AddAtIndex(int index, ItemData data, int q) 
        {
            inventory.TryAddAtIndex(index, data, q);
        }

    } 
}