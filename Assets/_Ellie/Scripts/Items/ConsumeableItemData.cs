using System.Collections.Generic;
using UnityEngine;

namespace CarGame
{
    [CreateAssetMenu(menuName = "Car/Item/New Consumable Item")]
    public class ConsumeableItemData : ItemData
    {
        [Header("Consumable")]
        public AudioClip useClip;

        public enum EffectType 
        {
            RestoreHealth,
            RestoreHunger,
            RestoreTurbo,
        }

        [System.Serializable]
        public class Effect 
        {
            public EffectType type;
            public int amount;
        }

        [SerializeField] private List<Effect> effects;

        public List<Effect> Effects => effects;
        
    }
}
