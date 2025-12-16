using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CarGame
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private InventorySlotUI[] inventorySlots;

        public event Action<InventorySlotUI, InventorySlotUI> AnyValueChanged = delegate { };
        public event Action<InventorySlotUI, Vector3> ItemDropped = delegate { };
        public event Action<InventorySlotUI, int> ItemQuantityChanged = delegate { };
        public event Action<InventorySlotUI> OnItemChanged = delegate { };


        protected InventoryPanelUI inventoryPanelU;
        public void Init(InventoryPanelUI inventoryPanelUI)
        {
            inventoryPanelU = inventoryPanelUI;

            InitInventorySlots();
        }

        private void InitInventorySlots() 
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                inventorySlots[i].Init(this, i);
            }
        }


        public void Refresh(InventoryItem[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                inventorySlots[i].Setup(items[i]);
            }

            if (inventoryPanelU)
                inventoryPanelU.OnRefresh();
        }

        public void OnSlotClicked(InventorySlotUI clickedSlot, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left) 
            {
                HandleLeftClick(clickedSlot, eventData);
            }
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                HandleRightClick(clickedSlot, eventData);
            }
        }
        protected virtual bool IsValidSlot(InventorySlotUI clickedSlot, InventorySlotUI clonedSlot) 
        {
            if (clickedSlot.Type == InventorySlotUI.SlotType.Gadget) 
            {
                var item = clonedSlot.SlottedItem;
                if (item.ItemData.GetType() == typeof(BombItem) && clickedSlot.allowedItem == InventorySlotUI.AllowedItem.Bomb) 
                {
                    return true;
                }
            }
            else 
            {
                return true;
            }

            return false;
        }

        private void HandleLeftClick(InventorySlotUI clickedSlot, PointerEventData eventData) 
        {
            /*
            if (clickedSlot.SlottedItem != null && clickedSlot.SlottedItem.ItemData.GetType() == typeof(WeaponItemData))
            {
                Debug.LogWarning("Disabled");
                return;
            }
            */


            if (inventoryPanelU.ClonedSlot.SlottedItem == null)
            {
                if (clickedSlot.SlottedItem == null)
                    return;

                inventoryPanelU.OnItemSelected();

                inventoryPanelU.ShowClone(clickedSlot.SlottedItem);
                //ItemQuantityChanged.Invoke(clickedSlot, clickedSlot.SlottedItem.Quantity);
                OnItemChanged.Invoke(clickedSlot);
            }
            else
            {
                if (!IsValidSlot(clickedSlot, inventoryPanelU.ClonedSlot))
                {
                    return;
                }

                inventoryPanelU.OnItemPlaced();

                if (clickedSlot.SlottedItem == inventoryPanelU.ClonedSlot.SlottedItem)
                {
                    AnyValueChanged.Invoke(inventoryPanelU.ClonedSlot, clickedSlot);
                    inventoryPanelU.RemoveClone();
                }
                else
                {

                    if (clickedSlot.allowedItem == InventorySlotUI.AllowedItem.Bomb) 
                    {
                        inventoryPanelU.OnBombPlaced();
                    }

                    AnyValueChanged.Invoke(inventoryPanelU.ClonedSlot, clickedSlot);
                    //inventoryPanelU.RemoveClone();
                }
            }
        }
        public void HandleLeftOver(InventoryItem item) 
        {
            inventoryPanelU.ShowClone(item);
        }

        private void HandleRightClick(InventorySlotUI clickedSlot, PointerEventData eventData)
        {
            if (clickedSlot.SlottedItem == null)
            {
                return;
            }

            if (clickedSlot.SlottedItem.ItemData.GetType() == typeof(WeaponItemData))
            {
                Debug.LogWarning("Disabled");
                return;
            }

            if (inventoryPanelU.ClonedSlot.SlottedItem == null)
            {
                if (clickedSlot.SlottedItem == null)
                    return;

                inventoryPanelU.ShowClone(new InventoryItem(clickedSlot.SlottedItem.ItemData, 1));
                
                inventoryPanelU.OnItemSplit();
                ItemQuantityChanged.Invoke(clickedSlot, 1);
            }
            else
            {
                if (!IsValidSlot(clickedSlot, inventoryPanelU.ClonedSlot))
                {
                    return;
                }

                if (clickedSlot.SlottedItem.ItemData == inventoryPanelU.ClonedSlot.SlottedItem.ItemData)
                {
                    ItemQuantityChanged.Invoke(clickedSlot, 1);
                    inventoryPanelU.OnItemSplit();
                    inventoryPanelU.ClonedSlot.SlottedItem.Quantity++;
                    inventoryPanelU.ClonedSlot.Refresh();
                }
            }
        }

        public void DropItemAtIndex(InventorySlotUI clickedSlot, Vector2 pos) 
        {
            ItemDropped.Invoke(clickedSlot, pos);
        }

        public void OnItemPickup()
        {
            inventoryPanelU.OnItemPickup();
        }

        public void OnItemBreak()
        {
            inventoryPanelU.OnItemBreak();
        }
    }
}