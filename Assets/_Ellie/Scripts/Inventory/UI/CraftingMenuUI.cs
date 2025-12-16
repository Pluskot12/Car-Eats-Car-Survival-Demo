using PrimeTween;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CarGame
{
    public class CraftingMenuUI : MonoBehaviour
    {
        [Header("Crafting")]
        //[SerializeField] private CraftingTab[] categories;
        [SerializeField] private CraftingRecipeList recipeList;
        [SerializeField] private CraftingListUI craftingList;
        [SerializeField] private CraftingRecipePanelUI craftingPanel;

        [Header("Animation Settings")]
        [SerializeField] private RectTransform parent;
        [SerializeField] private float offPosition = -500f;
        [SerializeField] private float inDuration = 0.2f;

        bool isShowing;
        
        private void Start()
        {
            craftingList.Setup(recipeList);

            PlayerInventory.Instance.InventoryController.AnyValueChanged += InventoryController_AnyValueChanged;
        }

        private void InventoryController_AnyValueChanged(InventoryItem[] obj)
        {
            craftingPanel.UpdateSlots();
        }

        public void Show(bool show, bool animate = true)
        {
            isShowing = show;

            if (animate)
            {
                Animate();
            }
            else 
            {
                parent.anchoredPosition = new Vector2(offPosition, 0);
            }
        }

        private void Animate()
        {
            Tween.UIAnchoredPositionX(parent, endValue: isShowing ? 0 : offPosition, duration: inDuration, ease: Ease.InOutQuart);
        }

        public void UpdateCraftingPanel(CraftingRecipe recipe)
        {
            craftingPanel.UpdatePanel(recipe);
        }
    }
}
