
using UnityEngine;

namespace CarGame
{
    [CreateAssetMenu(menuName = "Car/Item/New Building")]
    public class BuildingItem : ItemData
    {
        public Building prefab;
        public float maxAngle;
        public AudioClip placementSound;
    }
}
