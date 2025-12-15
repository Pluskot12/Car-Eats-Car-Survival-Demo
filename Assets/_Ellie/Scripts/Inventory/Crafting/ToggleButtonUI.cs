using UnityEngine;
using UnityEngine.UI;

namespace CarGame
{
    public class ToggleButtonUI : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Sprite onSprite;
        [SerializeField] private Sprite offSprite;
        [SerializeField] private CraftingRecipe.Type type;

        public CraftingRecipe.Type Type => type;

        private bool active;

        
        private CraftingListUI craftingList;

        public void Setup(CraftingListUI crafingList, bool active) 
        {
            this.craftingList = crafingList;
            this.active = active;

            SetActive(active);
        }

        public void OnButtonPress()
        {
            if (active)
                return;

            craftingList.OnCategoryButton(type);

            SetActive(true);
        }

        public void SetActive(bool active) 
        {
            this.active = active;
            image.sprite = active ? onSprite : offSprite;
        }
    }
}
