using UnityEngine;

namespace CarGame
{
    [CreateAssetMenu(menuName = "Car/New Recipe")]
    public class CraftingRecipe : ScriptableObject
    {
        public ItemData item;
        public int quantity = 1;
        public Type type;

        public Ingredient[] ingredients;

        [System.Serializable]
        public struct Ingredient 
        {
            public ItemData item;
            public int quantity;
        }

        public enum Type 
        {
            None,
            Building,
            Refine,
            Tool,
            Weapon
        }
    }
}
