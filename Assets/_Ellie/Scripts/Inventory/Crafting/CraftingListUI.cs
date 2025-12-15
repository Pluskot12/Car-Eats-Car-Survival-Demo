using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CarGame
{
    public class CraftingListUI : MonoBehaviour
    {
        [SerializeField] private CraftingMenuUI craftingMenu;
        [SerializeField] private ToggleButtonUI[] categoryButtons;
        [SerializeField] private CraftingGridItemUI[] gridItems;
        [SerializeField] private TextMeshProUGUI pageLabel;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip clickSound;

        private CraftingRecipeList recipeList;

        CraftingRecipe.Type currentCategory;
        CraftingRecipe currentRecipe;

        int page = 1;
        int totalPages = 1;

        public int SlotCount => gridItems.Length;

        public void Setup(CraftingRecipeList recipeList)
        {
            this.recipeList = recipeList;

            foreach (var button in categoryButtons)
            {
                if (button.Type == currentCategory) 
                {
                    button.Setup(this, true);
                }
                else 
                {
                    button.Setup(this, false);
                }
            }

            ChangeCategory(CraftingRecipe.Type.None);

            craftingMenu.UpdateCraftingPanel(recipeList.Recipes[0]);
        }
        
        public void OnCategoryButton(CraftingRecipe.Type type)
        {
            foreach (var button in categoryButtons)
            {
                button.SetActive(false);
            }

            audioSource.PlayOneShot(clickSound);

            ChangeCategory(type);

        }

        private void ChangeCategory(CraftingRecipe.Type type) 
        {
            currentCategory = type;

            page = 1;
            pageLabel.text = page.ToString();

            int totalRecipes = recipeList.GetRecipeCountByType(type);
            totalPages = Mathf.CeilToInt(totalRecipes / (float)SlotCount);

            List<CraftingRecipe> recipes = GetRecipePagedList(type, 1);

            UpdateList(recipes);
        }

        private List<CraftingRecipe> GetRecipePagedList(CraftingRecipe.Type type, int page) 
        {
            int startIndex = (page - 1) * SlotCount;

            if (type == CraftingRecipe.Type.None)
            {
                return recipeList.GetRecipesPaged(startIndex, SlotCount);
            }
            else
            {
                return recipeList.GetRecipesByTypePaged(type, startIndex, SlotCount);
            }
        }
        
        public void OnArrowPress(int nextPage) 
        {
            page += nextPage;

            if (page < 1) 
            {
                page = 1; 
                return;
            }
            if (page > totalPages)
            {
                page = totalPages;
                return;
            }

            audioSource.PlayOneShot(clickSound);

            pageLabel.text = page.ToString();

            UpdateList(GetRecipePagedList(currentCategory, page));
        }

        public void OnSlotPressed(CraftingRecipe recipe) 
        {
            if (recipe == null || recipe == currentRecipe)
            {
                return;
            }

            audioSource.PlayOneShot(clickSound);

            currentRecipe = recipe;
            craftingMenu.UpdateCraftingPanel(recipe);
        }

        public void UpdateList(List<CraftingRecipe> recipes) 
        {
            int gridSize = gridItems.Length;

            for (int i = 0; i < gridSize; i++) 
            {
                if (i < recipes.Count)
                {
                    gridItems[i].Setup(recipes[i]);
                }
                else 
                {
                    gridItems[i].Setup(null);
                }
            }
        }
    }
}
