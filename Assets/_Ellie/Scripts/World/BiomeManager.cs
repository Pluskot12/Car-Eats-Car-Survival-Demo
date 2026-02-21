using UnityEngine;
using UnityEngine.Events;

namespace CarGame
{
    public class BiomeManager : MonoBehaviour
    {
        public static BiomeManager Instance { get; private set; }

        [SerializeField] private BiomeData defaultBiome;

        private BiomeType currentBiomeType = BiomeType.CozyWoodlands;

        private BiomeData currentBiome;
        public BiomeData CurrentBiome => currentBiome;

        public UnityEvent<BiomeData> OnBiomeChanged;

        private void Awake()
        {
            Instance = this;

            currentBiome = defaultBiome;
        }

        public void OnBiomeChange(Biome biome) 
        {
            if (biome.Data == null) 
            {
                Debug.LogWarning("No BiomeData set for " + biome.gameObject.name);
                return;
            }

            if (biome.Type == BiomeType.DeadEnd) 
            {
                return;
            }

            if (currentBiomeType != biome.Type)
            {
                currentBiome = biome.Data;
                currentBiomeType = biome.Type;

                OnBiomeChanged.Invoke(currentBiome);
            }
        }
    }
}
