using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
namespace CarGame
{
    public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private CanvasGroup cg;
        [SerializeField] private Image itemParent;
        [SerializeField] private Image itemIcon;
        [SerializeField] private Image frame;
        [SerializeField] private TextMeshProUGUI quantityText;
        [SerializeField] private InventoryItem slottedItem;
        InventoryUI ui;
       
        public GadgetBarUI gadgetBar;
        public int Index { get; private set; }

        public RectTransform RectTransform => rect;
        public CanvasGroup CanvasGroup => cg;
        public Image Frame => frame;
        public InventoryItem SlottedItem => slottedItem;
        public Transform ItemParent => itemParent.transform;

        public AllowedItem allowedItem = AllowedItem.All;

        public enum SlotType 
        {
            Inventory,
            ActionBar,
            Gadget
        }

        public enum AllowedItem 
        {
            All,
            Bomb
        }

        public SlotType Type = SlotType.Inventory;

        public void Setup(InventoryItem item)
        {
            slottedItem = item;

            if (item == null)
            {
                itemParent.gameObject.SetActive(false);
                itemIcon.sprite = null;
                quantityText.text = "";
            }
            else 
            {
                itemIcon.sprite = item.ItemData.sprite;
                itemParent.gameObject.SetActive(true);

                if (item.Quantity > 1 || item.ItemData.GetType() == typeof(WeaponItemData))
                {
                    quantityText.text = item.Quantity.ToString();
                }
                else 
                {
                    quantityText.text = "";
                }
            }
        }

        public void Refresh() 
        {
            Setup(slottedItem);
        }

        public void Init(InventoryUI ui, int i)
        {
            this.ui = ui;
            Index = i;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Type == SlotType.Gadget) 
            {
                gadgetBar.OnSlotClicked(this, eventData);
            }
            else
            {
                ui?.OnSlotClicked(this, eventData);
            }
                
        }
    }
}