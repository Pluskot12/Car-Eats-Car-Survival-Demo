using UnityEngine;

namespace CarGame
{
    [CreateAssetMenu(menuName = "Car/Item/New Tool")]
    public class ToolItemData : ItemData, IBreakable
    {
        public Tool prefab;
        public int maxDurability = 1000;

        public int MaxDurability => maxDurability;
    }
}
