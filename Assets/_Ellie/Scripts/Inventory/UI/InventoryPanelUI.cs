using PrimeTween;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CarGame
{
    public class InventoryPanelUI : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private GraphicRaycaster raycaster;

        [SerializeField] private InventoryUI inventory;
        [SerializeField] private ActionBarUI actionBar;
        [SerializeField] private InventoryUI gadgetBar;

        [SerializeField] private RectTransform inventoryRect;
        [SerializeField] private RectTransform inventoryParent;
        [SerializeField] private InventorySlotUI inventorySlotPrefab;
        [SerializeField] private Canvas rootCanvas;

        [Header("Crafting")]
        [SerializeField] private CraftingMenuUI craftingListPanel;
        [SerializeField] private CraftingRecipePanelUI craftingRecipePanel;

        [Header("Inventory Button")]
        [SerializeField] private Image inventoryButton;
        [SerializeField] private Sprite inventoryButtonOpen;
        [SerializeField] private Sprite inventoryButtonClose;

        [Header("Animation Settings")]
        [SerializeField] private float offPosition = -270f;
        [SerializeField] private float inDuration = 0.2f;

        [Header("Sounds")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip openAudio;
        [SerializeField] private AudioClip closeAudio;
        [SerializeField] private AudioClip selectAudio;
        [SerializeField] private AudioClip splitAudio;
        [SerializeField] private AudioClip placeAudio;
        [SerializeField] private AudioClip throwAudio;
        [SerializeField] private AudioClip bombPlacementSound;
        [SerializeField] private AudioClip itemBreakAudio;

        private Camera cam;

        private bool isShowing;

        private void Start()
        {
            cam = Camera.main;

            inventoryParent.anchoredPosition = new Vector2(0, offPosition);
            ShowInventory(isShowing, false);
            craftingListPanel.Show(isShowing, false);
            craftingRecipePanel.Show(isShowing, false);

            inventory.Init(this);
            gadgetBar.Init(this);
            //actionBar.Init(this);

            CreateClone();
        }

        public void Show()
        {
            canvas.enabled = true;
        }

        public void Hide()
        {
            canvas.enabled = false;
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.T))
            {
                if (clonedSlot.SlottedItem != null)
                {
                    DropItem(clonedSlot, Input.mousePosition);
                    RemoveClone();
                }
                else 
                {
                    var slotToDrop = actionBar.ActiveSlot();
                    if (slotToDrop.SlottedItem != null)
                    {
                        DropItem(slotToDrop, Input.mousePosition);
                        inventory.DropItemAtIndex(slotToDrop, Input.mousePosition);
                    }
                }

            }


            if (clonedSlot.SlottedItem != null)
            {
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rootCanvas.transform as RectTransform,  Input.mousePosition, rootCanvas.worldCamera, out pos);
                clonedSlot.transform.localPosition = pos;

                if (Input.GetMouseButtonDown(0))
                {
                    bool isInside = IsInsideInventoryRect(Input.mousePosition);

                    if (!isInside)
                    {
                        DropItem(clonedSlot, Input.mousePosition);
                    }
                }
            }
        }

        private void UpdateButtonSprite()
        {
            if (isShowing)
            {
                inventoryButton.sprite = inventoryButtonClose;
            }
            else
            {
                inventoryButton.sprite = inventoryButtonOpen;
            }
        }

        public void OnInventoryButton()
        {
            isShowing = !isShowing;

            Tween.PunchScale(inventoryButton.transform, strength: Vector3.one * 0.2f, duration: .3f, frequency: 7);

            ShowInventory(isShowing);

            craftingListPanel.Show(isShowing);
            craftingRecipePanel.Show(isShowing);
        }

        private void ShowInventory(bool show, bool animate = true)
        {
            UpdateButtonSprite();

            if (animate)
            {
                audioSource.PlayOneShot(show ? openAudio : closeAudio);

                Animate();

                GameManager.Instance.Player.OnInventory(show);
            }
        }

        private void Animate()
        {
            Tween.UIAnchoredPositionY(inventoryParent, endValue: isShowing ? 0 : offPosition, duration: inDuration, ease: Ease.InOutQuart);
        }

        public bool IsInsideInventoryRect(PointerEventData eventData)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(
                inventoryRect,
                eventData.position,
                eventData.pressEventCamera
            );
        }

        public bool IsInsideInventoryRect(Vector2 mousePosition)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(
                inventoryRect,
                mousePosition
            );
        }

        public void DropItem(InventorySlotUI inventorySlotUI, Vector2 screenPos)
        {
            audioSource.PlayOneShot(throwAudio);

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(screenPos);
            mouseWorld.z = 0f;
            Vector2 direction = (mouseWorld - GameManager.Instance.Player.transform.position).normalized;

            OnItemDropped(inventorySlotUI, direction);

            RemoveClone();
        }


        public void OnItemDropped(InventorySlotUI item, Vector3 force)
        {
            Vector3 position = GameManager.Instance.Player.transform.position;

            ItemSpawner.Instance.DropItem(item.SlottedItem.ItemData, item.SlottedItem.Quantity, position, force, true);
        }

        private InventorySlotUI clonedSlot;
        public InventorySlotUI ClonedSlot => clonedSlot;

        private void CreateClone()
        {
            clonedSlot = Instantiate(inventorySlotPrefab, transform.root);
            clonedSlot.Frame.enabled = false;
            clonedSlot.CanvasGroup.blocksRaycasts = false;
            clonedSlot.transform.localScale *= 1.15f;
            clonedSlot.Setup(null);
        }

        public void ShowClone(InventoryItem slot) 
        {
            var slottedItem = new InventoryItem(slot.ItemData, slot.Quantity);
            clonedSlot.gameObject.SetActive(true);
            clonedSlot.transform.SetAsLastSibling();

            clonedSlot.Setup(slottedItem);

            UIMananger.IsHoldingItem = slottedItem != null;
        }

        public void RemoveClone()
        {
            clonedSlot.Setup(null);
            clonedSlot.gameObject.SetActive(false);

            StartCoroutine(RenableShooting());
        }

        private IEnumerator RenableShooting() 
        {
            yield return new WaitForSeconds(0.1f);

            UIMananger.IsHoldingItem = false;
        }

        
        public void OnItemPickup()
        {
            audioSource.PlayOneShot(placeAudio);
        }

        public void OnRefresh()
        {
            actionBar.OnInventoryRefreshed();
        }
        public void SetInteractable(bool enable)
        {
            raycaster.enabled = enable;
        }

        public void OnBombPlaced()
        {
            audioSource.PlayOneShot(bombPlacementSound);
        }

        public void OnItemSelected()
        {
            audioSource.PlayOneShot(selectAudio);
        }

        public void OnItemPlaced() 
        {
            audioSource.PlayOneShot(placeAudio);
        }

        public void OnItemSplit()
        {
            audioSource.PlayOneShot(splitAudio);
        }

        internal void OnItemBreak()
        {
            audioSource.PlayOneShot(itemBreakAudio);
        }
    }
}
