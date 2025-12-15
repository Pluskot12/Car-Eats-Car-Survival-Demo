using UnityEngine;

namespace CarGame
{
    [CreateAssetMenu(menuName = "Car/Item/New Bomb")]
    public class BombItem : ItemData
    {
        public Bomb prefab;
        public int damage = 15;
        public float cooldown = 5;
    }
}
