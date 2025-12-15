using UnityEngine;

namespace CarGame
{
    [CreateAssetMenu(menuName = "Car/Item/New Weapon")]
    public class WeaponItemData : ItemData, IBreakable
    {
        public Gun gun;
        public ItemData ammoType;
        public int maxDurability = 100;

        public int MaxDurability => maxDurability;
    }
}
