using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CarGame
{
    public class CraftingGridItemUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image background;
        [SerializeField] private Image icon;

        public UnityEvent<CraftingRecipe> OnPress;

        private CraftingRecipe recipe;

        public void Setup(CraftingRecipe recipe) 
        {
            this.recipe = recipe;

            if (recipe == null)
            {
                background.gameObject.SetActive(false);
            }
            else 
            {
                icon.sprite = recipe.item.sprite;
                icon.gameObject.SetActive(true);

                background.gameObject.SetActive(true);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnPress.Invoke(recipe);
        }

    }
}
