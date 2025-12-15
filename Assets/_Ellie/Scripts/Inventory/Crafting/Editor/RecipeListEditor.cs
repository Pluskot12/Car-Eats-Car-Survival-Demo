#if UNITY_EDITOR

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CarGame
{
    [CustomEditor(typeof(CraftingRecipeList))]
    public class RecipeListEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            CraftingRecipeList recipeList = (CraftingRecipeList)target;

            GUIStyle bigButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                fixedHeight = 40
            };

            if (GUILayout.Button("Update Recipes", bigButtonStyle))
            {
                AutoFillRecipes(recipeList);
            }

            DrawDefaultInspector();
        }

        private void AutoFillRecipes(CraftingRecipeList recipeList)
        {
            string rootPath = recipeList.Path;

            string[] guids = AssetDatabase.FindAssets("t:CraftingRecipe", new[] { rootPath });
            var recipes = guids
                .Select(guid => AssetDatabase.LoadAssetAtPath<CraftingRecipe>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(r => r != null)
                .ToList();

            recipeList.UpdateRecipeList(recipes);

            EditorUtility.SetDirty(recipeList);
            AssetDatabase.SaveAssets();

            Debug.Log("Updated Recipe List. " + recipes.Count + " items found.");
        }
    }
}
#endif