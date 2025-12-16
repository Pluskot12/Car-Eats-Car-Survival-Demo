using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CarGame
{
    public class CraftingIngredientItemUI : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI quantityText;
        [SerializeField] private Sprite validSprite;
        [SerializeField] private Sprite invalidSprite;

        public void Setup(CraftingRecipe.Ingredient ingredient, bool valid)
        {
            background.sprite = valid ? validSprite : invalidSprite;

            icon.sprite = ingredient.item.sprite;
            icon.gameObject.SetActive(true);

            quantityText.text = ingredient.quantity.ToString();

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
