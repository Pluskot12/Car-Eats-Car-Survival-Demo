using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarGame
{
    [CreateAssetMenu(menuName = "Car/New Recipe List")]
    public class CraftingRecipeList : ScriptableObject
    {
        [SerializeField] private string path;
        [SerializeField] List<CraftingRecipe> recipes;

        public string Path => path;
        public List<CraftingRecipe> Recipes => recipes;

        public void UpdateRecipeList(List<CraftingRecipe> recipes)
        {
            this.recipes = recipes;
        }

        public int GetRecipeCountByType(CraftingRecipe.Type type)
        {
            if (type == CraftingRecipe.Type.None)
            {
                return recipes.Count;
            }

            return recipes.Where(r => r != null && r.type == type).Count();
        }

        public List<CraftingRecipe> GetRecipesByType(CraftingRecipe.Type type)
        {

            return recipes.Where(r => r != null && r.type == type).ToList();
        }

        public List<CraftingRecipe> GetRecipesByTypePaged(CraftingRecipe.Type type, int startIndex, int count)
        {
            var filtered = GetRecipesByType(type);

            if (startIndex < 0)
                startIndex = 0;

            if (startIndex >= filtered.Count) 
                return new List<CraftingRecipe>();

            return filtered.Skip(startIndex).Take(count).ToList();
        }

        public List<CraftingRecipe> GetRecipesPaged(int startIndex, int count = 5)
        {
            var validRecipes = recipes.Where(r => r != null).ToList();

            if (startIndex < 0) 
                startIndex = 0;

            if (startIndex >= validRecipes.Count) 
                return new List<CraftingRecipe>();

            return validRecipes.Skip(startIndex).Take(count).ToList();
        }
    }
}
