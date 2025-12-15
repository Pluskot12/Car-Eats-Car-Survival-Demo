using System;
using System.Collections;
using UnityEngine;

namespace CarGame
{
    public class ActionBarUI : MonoBehaviour
    {
        [SerializeField] InventorySlotUI[] slots;
        [SerializeField] RectTransform selector;
        [SerializeField] InventoryUI ui;
        [SerializeField] PlayerStatPanelUI statPanel;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip scrollSound;
        [SerializeField] private float scrollSoundCooldown = 0.1f;

        bool canPlay = true;

        private int selectedSlot = -1;

        private void Start()
        {
            ChangeSlot(0, false);

            selector.SetParent(slots[selectedSlot].Frame.transform);
            selector.SetAsFirstSibling();

            selector.localPosition = Vector2.zero;
        }

        private void Update()
        {
            if (GameManager.Instance.Player.IsDead)
                return;

            if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeSlot(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeSlot(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeSlot(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeSlot(3);
            if (Input.GetKeyDown(KeyCode.Alpha5)) ChangeSlot(4);
            
            if (Input.mouseScrollDelta.y > 0) 
            {
                ChangeSlot(selectedSlot - 1);
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                ChangeSlot(selectedSlot + 1);
            }

            if (Input.GetMouseButtonDown(0)) 
            {
                TryUseItem();
            }
        }

        private void TryUseItem() 
        {
            if (UIMananger.IsPointerOverUIObject()) 
            {
                return;
            }

            if (UIMananger.IsHoldingItem)
            {
                return;
            }

            if (slots[selectedSlot].SlottedItem == null) 
            {
                return;
            }

            if (slots[selectedSlot].SlottedItem.ItemData.GetType() == typeof(ConsumeableItemData)) 
            {
                GameManager.Instance.Player.ApplyItemEffect((ConsumeableItemData)slots[selectedSlot].SlottedItem.ItemData);
                PlayerInventory.Instance.InventoryController.OnItemUse(selectedSlot); 
            }
        }

        public InventorySlotUI ActiveSlot() 
        {
            return slots[selectedSlot];
        }

        private void ChangeSlot(int slot, bool playSound = true) 
        {
            if (slot == selectedSlot)
                return;

            if (slot >=  slots.Length)
                slot = 0;
            else if (slot < 0)
                slot = slots.Length - 1;


            if (canPlay && playSound) 
            { 
                audioSource.PlayOneShot(scrollSound);
                StartCoroutine(SoundCooldown());
            }

            selectedSlot = slot;
            selector.SetParent(slots[selectedSlot].Frame.transform);
            selector.SetAsFirstSibling();
            selector.localPosition = Vector2.zero;



            OnInventoryRefreshed(playSound);
        }

        public void OnInventoryRefreshed(bool playSound = true) 
        {
            UpdateAttachment(playSound);
            UpdateText();
        }

        private void UpdateAttachment(bool playSound) 
        {
            if (slots[selectedSlot].SlottedItem == null)
            {
                GameManager.Instance.Player.Attachments.AttachItem(null, -1, true);
                //ammoText.text = "";
                statPanel.UpdateAmmo("");
                statPanel.UpdateDurability("");
            }
            else
            {
                GameManager.Instance.Player.Attachments.AttachItem(slots[selectedSlot].SlottedItem.ItemData, selectedSlot, playSound);
            }
        }

        private void UpdateText() 
        {
            string ammoText = "";
            string durabilityText = "";

            if (slots[selectedSlot].SlottedItem != null) 
            {
                if (slots[selectedSlot].SlottedItem.ItemData.GetType() == typeof(WeaponItemData)) 
                {
                    var weapon = (WeaponItemData)slots[selectedSlot].SlottedItem.ItemData;
                    int ammoCountInInventory = PlayerInventory.Instance.InventoryController.Inventory.GetItemCount(weapon.ammoType);
                    ammoText = slots[selectedSlot].SlottedItem.Quantity + "/" + ammoCountInInventory;

                }

                if (slots[selectedSlot].SlottedItem.ItemData is IBreakable breakable)
                {
                    durabilityText = "" + slots[selectedSlot].SlottedItem.Durability;
                }
            }



            
            //ammoText.text = text;

            statPanel.UpdateAmmo(ammoText);
            statPanel.UpdateDurability(durabilityText);
        }

        IEnumerator SoundCooldown() 
        {
            canPlay = false;

            yield return new WaitForSeconds(scrollSoundCooldown);

            canPlay = true;
        }
    }
}
