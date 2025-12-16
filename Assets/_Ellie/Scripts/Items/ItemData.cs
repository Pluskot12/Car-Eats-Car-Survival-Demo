using UnityEngine;

namespace CarGame
{
    [CreateAssetMenu(menuName = "Car/Item/New Item")]
    public class ItemData : ScriptableObject
    {
        public string displayName;
        public Sprite sprite;
        public int maxStackSize = 1;
    }
}