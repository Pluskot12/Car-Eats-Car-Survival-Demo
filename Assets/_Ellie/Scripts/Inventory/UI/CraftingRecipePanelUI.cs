using PrimeTween;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CarGame.CraftingRecipe;

namespace CarGame
{
    public class CraftingRecipePanelUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Button craftButton;
        [SerializeField] private Sprite craftButtonValid;
        [SerializeField] private Sprite craftButtonInvalid;
        [SerializeField] private HorizontalLayoutGroup layoutGroup;
        [SerializeField] private CraftingIngredientItemUI[] slots;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip clickSound;
        [SerializeField] private AudioClip craftSound;

        [Header("Animation Settings")]
        [SerializeField] private RectTransform parent;
        [SerializeField] private float offPosition = 400f;
        [SerializeField] private float inDuration = 0.2f;

        bool isShowing;

        public void Show(bool show, bool animate = true)
        {
            isShowing = show;

            if (animate)
            {
                Animate();
            }
            else
            {
                parent.anchoredPosition = new Vector2(0, offPosition);
            }
        }
        CraftingRecipe currentRecipe;
        public void UpdatePanel(CraftingRecipe recipe)
        {
            if (recipe == null)
                return;

            currentRecipe = recipe;

            icon.sprite = recipe.item.sprite;
            nameText.text = recipe.item.displayName;

            UpdateSlots();
        }

        public void UpdateSlots() 
        {
            int validSlots = 0;

            for (int i = 0; i < slots.Length; i++) 
            {
                if (i < currentRecipe.ingredients.Length)
                {
                    bool valid = HaveEnoughOfItem(currentRecipe.ingredients[i]);
                    slots[i].Setup(currentRecipe.ingredients[i], valid);
                    if (valid) 
                    {
                        validSlots++;
                    }
                    //
                }
                else 
                {
                    slots[i].Hide();
                }
            }

            layoutGroup.padding.left = currentRecipe.ingredients.Length % 2;

            UpdateButton(validSlots == currentRecipe.ingredients.Length);
        }

        private bool HaveEnoughOfItem(Ingredient ingredient) 
        {
            return PlayerInventory.Instance.InventoryController.GetCountOfType(ingredient.item) >= ingredient.quantity;
        }

        public void OnCraftButton() 
        {
            int requried = currentRecipe.ingredients.Length;
            int valid = 0;

            foreach (var ingredient in currentRecipe.ingredients) 
            {
                if (HaveEnoughOfItem(ingredient)) 
                {
                    valid++;
                }
            }

            if (valid == requried)
            {
                Debug.Log("Crafting " + currentRecipe.item);

                foreach (var ingredient in currentRecipe.ingredients)
                {
                    PlayerInventory.Instance.InventoryController.RemoveItems(ingredient.item, ingredient.quantity);
                }

                if (PlayerInventory.Instance.InventoryController.CanFit(currentRecipe.item, currentRecipe.quantity))
                {
                    int leftover = PlayerInventory.Instance.InventoryController.OnItemPickup(currentRecipe.item, currentRecipe.quantity);

                    if (leftover > 0)
                    {
                        ItemSpawner.Instance.SpawnItem(currentRecipe.item, leftover, GameManager.Instance.Player.transform.position, Vector2.zero);
                    }
                }
                else 
                {
                    ItemSpawner.Instance.SpawnItem(currentRecipe.item, currentRecipe.quantity, GameManager.Instance.Player.transform.position, Vector2.zero);
                }

                audioSource.PlayOneShot(craftSound);
            }
            else 
            {
                Debug.LogWarning("Not enough items");
            }
        }

        private void UpdateButton(bool valid)
        {
            /*
            if (!PlayerInventory.Instance.InventoryController.CanFit(currentRecipe.item, currentRecipe.quantity)) 
            {
                valid = false;
            }
            */

            craftButton.enabled = valid;

            if (valid)
            {
                craftButton.image.sprite = craftButtonValid;
            }
            else 
            {
                craftButton.image.sprite = craftButtonInvalid;
            }
        }

        private void Animate()
        {
            Tween.UIAnchoredPositionY(parent, endValue: isShowing ? 0 : offPosition, duration: inDuration, ease: Ease.InOutQuart);
        }
    }
}
